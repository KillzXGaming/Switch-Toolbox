using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpYaml.Serialization;

namespace FirstPlugin
{
    public class MSYT
    {
        public static string ToYaml(MSBT msbt)
        {
            var header = MessageHeader.FromMSBT(msbt);
            var serializer = new Serializer();
            return serializer.Serialize(header);
        }

        public class MessageHeader
        {
            public List<MessageEntry> entries = new List<MessageEntry>();

            public static MessageHeader FromMSBT(MSBT msbt)
            {
                MessageHeader header = new MessageHeader();
                for (int i = 0; i < msbt.header.Label1.Labels.Count; i++) {
                    var entry = msbt.header.Label1.Labels[i];

                    var msgEntry = new MessageEntry();
                    msgEntry.Name = entry.Name;
                    msgEntry.contents.text.Add(entry.String.GetText(msbt.header.StringEncoding));
                    header.entries.Add(msgEntry);
                }
                return header;
            }
        }

        public class MessageEntry
        {
            public Content contents = new Content();

            [YamlIgnore]
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        public class Content
        {
            public List<string> text = new List<string>();
        }
    }
}
