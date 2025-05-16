using snapwatch.Internal.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace snapwatch.Internal.Service
{
    public class IndexService
    {
        private readonly Dictionary<ushort, uint> _pidx;
        private readonly Config _config;

        public IndexService()
        {
            this._config = new Config();
        }

        public Dictionary<ushort, uint> LoadPIDX()
        {
            var dict = new Dictionary<ushort, uint>();

            foreach (var line in File.ReadLines(this._config.ReturnConfig().MOVIES_PIDX_READ))
            {
                var parts = line.Split('\t');
                if (parts.Length == 2 && ushort.TryParse(parts[0], out ushort page) && uint.TryParse(parts[1], out uint offset))
                {
                    dict[page] = offset;
                }
            }

            return dict;
        }

        public uint GetNextOffset(ushort page)
        {
            ushort nextPage = (ushort)(page + 1);

            while (nextPage <= _pidx.Keys.Max())
            {
                if (_pidx.TryGetValue(nextPage, out uint nextOffset))
                {
                    return nextOffset;
                }

                nextPage++;
            }

            var fileInfo = new FileInfo(_config.ReturnConfig().MOVIES_JSON_READ);
            return (uint) fileInfo.Length;
        }
    }
}
