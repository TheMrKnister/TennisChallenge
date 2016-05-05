using System;
using System.IO;
using Microsoft.VisualBasic.FileIO;

//using Microsoft.VisualBasic.FileIO;

namespace TennisWeb.Services
{
    public class CsvFileReadService : IDisposable
    {
        private TextFieldParser _parser;
        private Stream _stream;
        private string[] _current;
        private readonly string[] _delimeters;

        public CsvFileReadService()
        {
            _delimeters = new[] { ";" };
        }

        public CsvFileReadService(params string[] delimiters)
        {
            _delimeters = delimiters;
        }

        public void Open(Stream fileStream, bool skipHeader = false)
        {
            if (_stream != null)
                _stream.Dispose();
            if (_parser != null)
                _parser.Dispose();

            _stream = fileStream;
            _parser = new TextFieldParser(_stream);
            _parser.CommentTokens = new string[] { "#" };
            _parser.SetDelimiters(_delimeters);
            _parser.HasFieldsEnclosedInQuotes = true;

            if (skipHeader)
            {
                MoveNext();
            }
            else
            {
                _current = _parser.ReadFields();
            }
        }

        public bool MoveNext()
        {
            var hasNext = !_parser.EndOfData;

            if (hasNext)
            {
                _current = _parser.ReadFields();
            }

            return hasNext;
        }

        public string[] Current
        {
            get { return _current; }
        }

        public void Dispose()
        {
            _stream.Close();
            _stream.Dispose();
            _parser.Close();
            _parser.Dispose();
        }
    }
}