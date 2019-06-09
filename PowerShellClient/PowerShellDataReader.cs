using System;
using System.Collections.Generic;
using System.IO;

namespace PowerShellClient
{
    public sealed class PowerShellDataReader : IDisposable
    {
        private readonly StreamReader _streamReader;
        private Dictionary<string, string> _currentRow;

        internal PowerShellDataReader(StreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        public bool Read()
        {
            var isFirstRead = _currentRow == null;
            _currentRow = new Dictionary<string, string>();

            while(!_streamReader.EndOfStream)
            {
                var field = _streamReader.ReadLine().Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                if (field.Length == 0)
                {
                    if(isFirstRead && _currentRow.Count == 0)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                if(field.Length == 1)
                {
                    throw new PowerShellException("Invalid Results Set. The command did not produce tabular data.");
                }

                if(field.Length > 2)
                {
                    _currentRow.Add(field[0].Trim(), RebuildSplitValue(field).Trim());
                }
                else
                {
                    _currentRow.Add(field[0].Trim(), field[1].Trim());
                }
            }

            return _currentRow.Count > 0;
        }

        private static string RebuildSplitValue(string[] field)
        {
            return string.Join(":", field, 1, field.Length - 1);
        }

        public string this[string columnName]
        {
            get { return GetValue(columnName); }
        }

        private string GetValue(string columnName)
        {
            if(_currentRow == null)
            {
                throw new InvalidOperationException($"{nameof(Read)}() must be called before trying to access the current row");
            }
            else if(_currentRow.Count == 0)
            {
                throw new InvalidOperationException("The command did not return a result.");
            }

            // Let the KeyNotFound exception be thrown from the dictionary.
            // It's slightly faster to not check and we were only going to throw an exception with a similar error message anyway.
            return _currentRow[columnName];
        }

        public void Dispose()
        {
            _streamReader.Dispose();
        }
    }
}
