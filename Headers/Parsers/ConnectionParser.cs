﻿using System;
using HttpServer.Tools;

namespace HttpServer.Headers.Parsers
{
    /// <summary>
    /// Parses <see cref="ConnectionHeader"/>.
    /// </summary>
    [ParserFor(ConnectionHeader.NAME)]
    public class ConnectionParser : IHeaderParser
    {
        #region IHeaderParser Members

        /// <summary>
        /// Parse a header
        /// </summary>
        /// <param name="name">Name of header.</param>
        /// <param name="reader">Reader containing value.</param>
        /// <returns>HTTP Header</returns>
        /// <exception cref="FormatException">Header value is not of the expected format.</exception>
        public IHeader Parse(string name, ITextReader reader)
        {
            string value = reader.ReadToEnd(",;");
            if (reader.Current == ',') // to get rid of the TE header.
                reader.ReadToEnd(';');

            ConnectionType type;
            if (string.Compare(value, "close", true) == 0)
                type = ConnectionType.Close;
            else if (value.StartsWith("keep-alive", StringComparison.CurrentCultureIgnoreCase))
                type = ConnectionType.KeepAlive;
            else if (value.StartsWith("te", StringComparison.CurrentCultureIgnoreCase))
                type = ConnectionType.TransferEncoding;
            else
                throw new FormatException("Unknown connection type '" + value + "'.");

            // got parameters
            if (reader.Current == ';')
            {
                HeaderParameterCollection parameters = HeaderParameterCollection.Parse(reader);
                return new ConnectionHeader(type, parameters);
            }

            return new ConnectionHeader(type);
        }

        #endregion
    }
}