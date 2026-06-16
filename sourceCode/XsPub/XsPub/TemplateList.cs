using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using XsPub.Utility;

namespace XsPub
{
    internal class TemplateList : List<DataTemplate>, IList
    {
        private bool _isReadOnly;
        private readonly Lazy<Dictionary<Type, DataTemplate>> _index;
        public Dictionary<Type, DataTemplate> Index
        {
            get
            {
                _isReadOnly = true;
                return _index.Value;
            }
        }

        public TemplateList()
        {
            _index = Lazy.Create(() => this.ToDictionary(template => template.DataType as Type));
        }

        bool IList.IsReadOnly
        {
            get { return _isReadOnly; }
        }
    }
}