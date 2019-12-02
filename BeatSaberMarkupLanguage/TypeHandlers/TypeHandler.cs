﻿using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Notify;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    public abstract class TypeHandler
    {
        public abstract Dictionary<string, string[]> Props { get; }
        public abstract void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams);
        public virtual void HandleTypeAfterChildren(ComponentTypeWithData componentType, BSMLParserParams parserParams) { }
    }

    public abstract class TypeHandler<T> : TypeHandler
        where T : Component
    {
        public abstract Dictionary<string, Action<T, string>> Setters { get; }

        //public virtual void HandleType(T obj, Dictionary<string, string> data, BSMLParserParams parserParams, Dictionary<string, PropertyInfo> propertyMap = null)
        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            if (componentType.component is T obj)
            {

                NotifyUpdater updater = null;
                if (parserParams.host is INotifiableHost notifyHost && (componentType.propertyMap?.Count ?? 0) > 0)
                {
                    updater = componentType.component.gameObject.GetComponent<NotifyUpdater>();
                    if (updater == null)
                    {
                        updater = componentType.component.gameObject.AddComponent<NotifyUpdater>();
                        updater.NotifyHost = notifyHost;
                    }
                }
                foreach (KeyValuePair<string, string> pair in componentType.data)
                {
                    if (Setters.TryGetValue(pair.Key, out Action<T, string> action))
                    {
                        action.Invoke(obj, pair.Value);
                        if (updater != null && componentType.propertyMap != null && componentType.propertyMap.TryGetValue(pair.Key, out PropertyInfo prop))
                        {
                            updater?.AddAction(prop.Name, val => action.Invoke(obj, val.ToString()));
                        }
                    }
                    else
                        Logger.log?.Warn($"Tag {pair.Key} not supported (or manually handled) for {componentType.component.GetType().Name}.{typeof(T).Name} ({componentType.component.name}) with {componentType.typeHandler.GetType().Name}");
                }
            }
        }
    }

    public class ComponentHandler : Attribute
    {
        public Type type;

        public ComponentHandler(Type type)
        {
            this.type = type;
        }
    }
}
