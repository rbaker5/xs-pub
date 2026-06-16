using System.Diagnostics.Contracts;

namespace XsPub.Runtime.Settings
{
    [ContractClass(typeof(ITransformationSettingContract))]
    public interface ITransformationSetting
    {
        bool ValueIsSet { get; }
        object CurrentValue { get; }
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }
        void Reset();
        ITransformationSetting CloneAsDefault();
    }

    [ContractClassFor(typeof(ITransformationSetting))]
    public sealed class ITransformationSettingContract : ITransformationSetting
    {
        public bool ValueIsSet
        {
            get { return false; }
        }

        public object CurrentValue
        {
            get { return null; }
        }

        public string Name
        {
            get
            {
                Contract.Ensures(Contract.Result<string>().IndexOfAny(new [] {'.', ' '}) == -1);
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                return default(string);
            }
        }

        public string DisplayName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                return default(string);
            }
        }

        public string Description
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return default(string);
            }
        }

        public void Reset()
        {
            
        }

        public ITransformationSetting CloneAsDefault()
        {
            Contract.Ensures(Contract.Result<ITransformationSetting>() != null);
            return null;
        }
    }
}