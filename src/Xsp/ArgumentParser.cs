using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xsp
{
    public class AgumentSetting
    {
        public string TransformationName { get; set; }
        public string SettingName { get; set; }
        public string Value { get; set; }
    }

    public class ArgumentParser
    {
        private readonly string[] _args;
        private readonly List<string> _messages = new List<string>();
        private bool _isValid;

        private string Usage =
            "Usage: xsp <input.xsd|input.wsdl> <output-dir> [TransformName.Setting=value ...]" + Environment.NewLine +
            Environment.NewLine +
            "  input    XSD schema or WSDL document to publish." + Environment.NewLine +
            "  output   Directory to write transformed output files into." + Environment.NewLine +
            Environment.NewLine +
            "Transformations (activate by passing their settings):" + Environment.NewLine +
            "  AllToSequence.AllToSequence=true    Replace xs:all with xs:sequence" + Environment.NewLine +
            "  InlineGroups.InlineGroups=true      Inline xs:group references" + Environment.NewLine +
            "  ExplicitForms.ExplicitForms=true    Emit explicit form defaults";
        public string CurrentMessage
        {
            get
            {
                if (_messages.Count == 0)
                    return Usage;
                return "Error:" + Environment.NewLine + string.Join(Environment.NewLine + "  ", _messages) + Environment.NewLine + Usage;
            }
        }

        public string InputFile { get; private set; }
        public string OutputPath { get; private set; }
        public List<AgumentSetting> ArgumentSettings { get; private set; }

        public ArgumentParser(string[] args)
        {
            _args = args;
            ArgumentSettings = new List<AgumentSetting>();
        }

        public void Parse()
        {
            _isValid = true;
            // Just usage.
            if (_args.Length == 0)
                throw new ArgumentParseException(CurrentMessage);
            if (_args.Length < 2)
                criticalFailure("Invalid number of arguments.");

            InputFile = _args[0];
            OutputPath = _args[1];

            foreach (var arg in _args.Skip(2))
            {
                var propertyAndValue = arg.Split('=');
                if (propertyAndValue.Length != 2)
                {
                    nonCriticalFailure("Invalid argument, form must be [tranformationName.]settingName=value\n    {0}", arg);
                    continue;
                }

                var property = propertyAndValue[0];
                var propertyNameAndTransformationName = property.Split('.');
                if (propertyNameAndTransformationName.Length == 1)
                    addDefaultSetting(propertyNameAndTransformationName[0], propertyAndValue[1]);
                else if (propertyNameAndTransformationName.Length == 2)
                    addSetting(propertyNameAndTransformationName[0], propertyNameAndTransformationName[1], propertyAndValue[1]);
                else
                    nonCriticalFailure("Invalid argument, form must be [tranformationName.]settingName=value\n    {0}", arg);
            }

            if (!_isValid)
                throw new ArgumentParseException(CurrentMessage);
        }

        private void addSetting(string transformationName, string settingName, string value)
        {
            ArgumentSettings.Add(new AgumentSetting {TransformationName = transformationName, SettingName = settingName, Value = value});
        }

        private void addDefaultSetting(string settingName, string value)
        {
            ArgumentSettings.Add(new AgumentSetting { SettingName = settingName, Value = value });
        }

        private void criticalFailure(string message, params string[] args)
        {
            _messages.Add(string.Format(message, args));
            throw new ArgumentParseException(CurrentMessage);
        }

        private void nonCriticalFailure(string message, params string[] args)
        {
            _isValid = false;
            _messages.Add(string.Format(message, args));
        }
    }

    public class ArgumentParseException : Exception
    {
        public ArgumentParseException(string message) : base(message)
        {
        }
    }
}
