using System;
using Newtonsoft.Json;

namespace MetroidDreadLibrary
{
    public class NoFormattingConverter : JsonConverter
    {
        [ThreadStatic]
        static bool cannotWrite;

        // Disables the converter in a thread-safe manner.
        bool CannotWrite { get { return cannotWrite; } set { cannotWrite = value; } }

        public override bool CanWrite { get { return !CannotWrite; } }

        public override bool CanRead { get { return false; } }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException(); // Should be applied as a property rather than included in the JsonSerializerSettings.Converters list.
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            using (new PushValue<bool>(true, () => CannotWrite, val => CannotWrite = val))
            using (new PushValue<Formatting>(Formatting.None, () => writer.Formatting, val => writer.Formatting = val))
            {
                serializer.Serialize(writer, value);
            }
        }
    }

    public struct PushValue<T> : IDisposable
    {
        Action<T> setValue;
        T oldValue;

        public PushValue(T value, Func<T> getValue, Action<T> setValue)
        {
            if (getValue == null || setValue == null)
                throw new ArgumentNullException();
            this.setValue = setValue;
            this.oldValue = getValue();
            setValue(value);
        }

        #region IDisposable Members

        // By using a disposable struct we avoid the overhead of allocating and freeing an instance of a finalizable class.
        public void Dispose()
        {
            if (setValue != null)
                setValue(oldValue);
        }

        #endregion
    }
}