using System;
using System.Collections.Generic;
using System.IO;

namespace PowerShellClient
{
    public sealed class PowerShellDataReader : IDisposable
    {
        private readonly StreamReader _streamReader;

        private string[] _currentRow;
        private Dictionary<string, int> _columnNameMap;

        internal PowerShellDataReader(StreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        public bool Read()
        {
            if (_columnNameMap == null)
            {
                GetColumnNameMap();

                // Discard the second line as it's just a partition.
                _ = _streamReader.ReadLine();
            }

            if (!_streamReader.EndOfStream)
            {
                _currentRow = _streamReader.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return _currentRow.Length != 0;
            }
            else
            {
                return false;
            }
        }

        private void GetColumnNameMap()
        {
            string[] columnNames = null;

            while (columnNames == null && !_streamReader.EndOfStream)
            {
                columnNames = _streamReader.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                columnNames = columnNames.Length == 0 ? null : columnNames;
            }

            if (columnNames == null)
            {
                throw new PowerShellException("Command ran successfully but had no output.");
            }

            _columnNameMap = new Dictionary<string, int>();

            for (int i = 0; i < columnNames.Length; i++)
            {
                _columnNameMap.Add(columnNames[i], i);
            }
        }

        public string this[string columnName]
        {
            get { return GetValue(columnName); }
        }

        private string GetValue(string columnName)
        {
            if(_columnNameMap == null)
            {
                throw new InvalidOperationException("Read() must be called before trying to access the current row");
            }

            // Let the KeyNotFound exception be thrown from the dictionary.
            // It's slightly faster to not check and we were only going to throw an exception with a similar error message anyway.
            return _currentRow[_columnNameMap[columnName]];
        }

        public void Dispose()
        {
            _streamReader.Dispose();
        }
    }
}
