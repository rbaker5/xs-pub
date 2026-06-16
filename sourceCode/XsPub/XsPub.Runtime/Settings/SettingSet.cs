using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;

namespace XsPub.Runtime.Settings
{
    public class SettingSet : IList<ITransformationSetting>, IList
    {
        public string Name { get; private set; }
        public string TransformationName { get; private set; }
        public string DisplayName { get; private set; }
        public string Description { get; private set; }

        private readonly SortedList<string, ITransformationSetting> _propertyValues;

        public SettingSet(string transformationName, string displayName, string description, string name, IEnumerable<ITransformationSetting> defaults)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(transformationName));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));
            Contract.Requires<ArgumentNullException>(displayName != null);
            Contract.Requires<ArgumentNullException>(description != null);
            Contract.Requires<ArgumentNullException>(defaults != null);

            TransformationName = transformationName;
            Name = name;
            DisplayName = displayName;
            Description = description;

            _propertyValues = new SortedList<string, ITransformationSetting>();
            foreach (var setting in defaults)
                _propertyValues.Add(setting.Name, setting.CloneAsDefault());
        }

        public SettingSet(string transformationName, string displayName, string description, IEnumerable<ITransformationSetting> defaults = null)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(transformationName));
            Contract.Requires<ArgumentNullException>(displayName != null);
            Contract.Requires<ArgumentNullException>(description != null);

            TransformationName = transformationName;
            Name = transformationName;
            DisplayName = displayName;
            Description = description;

            _propertyValues = new SortedList<string, ITransformationSetting>();
            // Singletons may not have any sub properties.
            if (defaults != null)
            {
                foreach (var setting in defaults)
                    _propertyValues.Add(setting.Name, setting.CloneAsDefault());
            }
        }

        public XElement ValuesToXElement()
        {
            var element = new XElement(TransformationName, new XAttribute("Name", Name),
                                       from setting in _propertyValues.Values
                                       where setting.ValueIsSet
                                       select new XElement(setting.Name, setting.CurrentValue));
            return element;
        }

        public void ReadValuesFrom(XElement setElement)
        {
            foreach (var child in setElement.Elements())
            {
                var propertyName = child.Name.LocalName;
                SetPropertyValueWithString(propertyName, child.Value);
            }
        }

        public int Count { get { return _propertyValues.Count; }}

        public bool IsValidSetting(string propertyName)
        {
            Contract.Requires<ArgumentNullException>(propertyName != null);

            return _propertyValues.ContainsKey(propertyName);
        }

        public T GetPropertyValue<T>(string propertyName)
        {
            Contract.Requires<ArgumentNullException>(propertyName != null);

            TransformationSetting<T> setting;
            if (!TryGetSetting(propertyName, out setting))
                throw new ArgumentException(string.Format("'{0}' is not recogonized as a valid setting name.", propertyName));

            return setting.CurrentValue;
        }

        public void SetPropertyValue<T>(string propertyName, T propertyValue)
        {
            Contract.Requires<ArgumentNullException>(propertyName != null);

            TransformationSetting<T> setting;
            if (!TryGetSetting(propertyName, out setting))
                throw new ArgumentException(string.Format("'{0}' is not recogonized as a valid setting name.", propertyName));

            setting.CurrentValue = propertyValue;
        }

        public void SetPropertyValueWithString(string propertyName, string propertyValue)
        {
            Contract.Requires<ArgumentNullException>(propertyName != null);

            ITransformationSetting setting;
            if (!_propertyValues.TryGetValue(propertyName, out setting))
                throw new ArgumentException(string.Format("'{0}' is not recogonized as a valid setting name.", propertyName));

            var stringSetting = setting as TransformationSetting<string>;
            if (stringSetting != null)
            {
                stringSetting.CurrentValue = propertyValue;
            }
            else
            {
                var valueType = setting.GetType().GetGenericArguments()[0];
                try
                {
                    setting.GetType().GetProperty("CurrentValue", valueType).SetValue(setting, Convert.ChangeType(propertyValue, valueType), null);
                }
                catch (InvalidCastException e)
                {
                    throw new InvalidCastException(string.Format("Property {0} is of type {1}, and the value {2} could not be converted to a valid value.", propertyName, valueType, propertyValue));
                }
            }
        }
        
        public bool TryGetSetting<T>(string propertyName, out TransformationSetting<T> settingMetadata)
        {
            Contract.Requires<ArgumentNullException>(propertyName != null);

            settingMetadata = null;
            ITransformationSetting setting;
            if (!_propertyValues.TryGetValue(propertyName, out setting))
                return false;

            settingMetadata = setting as TransformationSetting<T>;
            if (settingMetadata == null)
                throw new ArgumentException(string.Format("Property {0} is not of type {1}.", propertyName, typeof(T)));
            
            return true;
        }

        public bool TryGetSetting(string propertyName, out ITransformationSetting settingMetadata)
        {
            Contract.Requires<ArgumentNullException>(propertyName != null);
            return _propertyValues.TryGetValue(propertyName, out settingMetadata);
        }

        public void Reset()
        {
            foreach (var setting in _propertyValues.Values)
                setting.Reset();
        }

        public IEnumerable<string> SettingNames
        {
            get { return _propertyValues.Keys; }
        }

        public IEnumerable<ITransformationSetting> Values
        {
            get { return _propertyValues.Values; }
        }

        public IEnumerator<ITransformationSetting> GetEnumerator()
        {
            return _propertyValues.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region ICollection<ITransformationSetting> Members

        void ICollection<ITransformationSetting>.Add(ITransformationSetting item)
        {
            throw new NotImplementedException();
        }

        void ICollection<ITransformationSetting>.Clear()
        {
            Reset();
        }

        bool ICollection<ITransformationSetting>.Contains(ITransformationSetting item)
        {
            return IsValidSetting(item.Name);
        }

        void ICollection<ITransformationSetting>.CopyTo(ITransformationSetting[] array, int arrayIndex)
        {
            _propertyValues.Values.CopyTo(array, arrayIndex);
        }

        bool ICollection<ITransformationSetting>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<ITransformationSetting>.Remove(ITransformationSetting item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IList<ITransformationSetting> Members

        int IList<ITransformationSetting>.IndexOf(ITransformationSetting item)
        {
            return _propertyValues.IndexOfKey(item.Name);
        }

        void IList<ITransformationSetting>.Insert(int index, ITransformationSetting item)
        {
            throw new NotImplementedException();
        }

        void IList<ITransformationSetting>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        ITransformationSetting IList<ITransformationSetting>.this[int index]
        {
            get
            {
                return _propertyValues.Values[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Clear()
        {
            Reset();
        }

        bool IList.Contains(object value)
        {
            var setting = value as ITransformationSetting;
            if (setting == null)
                return false;

            return IsValidSetting(setting.Name);
        }

        int IList.IndexOf(object value)
        {
            var setting = value as ITransformationSetting;
            if (setting == null)
                return -1;

            return _propertyValues.IndexOfKey(setting.Name);
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        bool IList.IsFixedSize
        {
            get { return true; }
        }

        bool IList.IsReadOnly
        {
            get { return true; }
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get
            {
                return _propertyValues.Values[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_propertyValues.Values).CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)_propertyValues).SyncRoot; }
        }

        #endregion
    }
}