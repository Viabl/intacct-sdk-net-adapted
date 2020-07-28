/*
 * Copyright 2020 Sage Intacct, Inc.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"). You may not
 * use this file except in compliance with the License. You may obtain a copy 
 * of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * or in the "LICENSE" file accompanying this file. This file is distributed on 
 * an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either 
 * express or implied. See the License for the specific language governing 
 * permissions and limitations under the License.
 */

using System;
using System.Collections.Generic;
using Intacct.SDK.Functions.Common.NewQuery.QueryOrderBy;
using Intacct.SDK.Functions.Common.NewQuery.QuerySelect;
using Intacct.SDK.Xml;

namespace Intacct.SDK.Functions.Common.NewQuery
{
    public class QueryFunction : AbstractFunction
    {
        public List<ISelect> SelectFields { get; set; }
        public string ObjectName { get; set; }
        
        private string _docParId;
        
        public string DocParId
        {
            get { return _docParId; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "";
                }
                _docParId = value;
            }
        }
        
        public List<IOrder> OrderBy { get; set; }
        
        public QueryFunction(string controlId = null) : base(controlId)
        {
        }
        
        public bool? CaseInsensitive { get; set; }

        private int? _pageSize;

        public int? PageSize
        {
            get { return _pageSize; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("PageSize cannot be negative. Set PageSize greater than zero.");
                }

                _pageSize = value;
            }
        }

        private int? _offset { get; set; }

        public int? Offset
        {
            get { return _offset; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Offset cannot be negative. Set Offset to zero or greater than zero.");
                }

                _offset = value;
            }
        }

        public override void WriteXml(ref IaXmlWriter xml)
        {
            xml.WriteStartElement("function");
            xml.WriteAttribute("controlid", ControlId, true);

            xml.WriteStartElement("query");

            if (SelectFields == null || SelectFields.Count == 0)
            {
                throw new ArgumentException("Select fields are required for query; set through method SelectFields setter.");
            }
            
            xml.WriteStartElement("select");
            foreach (var field in SelectFields)
            {
                field.WriteXml(ref xml);
            }
            xml.WriteEndElement(); //select

            if (string.IsNullOrEmpty(ObjectName))
            {
                throw new ArgumentException("Object Name is required for query; set through method from setter.");
            }
            
            xml.WriteElement("object", ObjectName, true);
            
            if (string.IsNullOrEmpty(DocParId) == false)
            {
                xml.WriteElement("docparid", DocParId, false);
            }

            if (OrderBy != null && OrderBy.Count > 0)
            {
                xml.WriteStartElement("orderby");
                foreach (var order in OrderBy)
                {
                    order.WriteXml(ref xml);
                }
                xml.WriteEndElement(); //orderby
            }

            if (CaseInsensitive != null)
            {
                xml.WriteStartElement("options");
                xml.WriteElement("caseinsensitive", CaseInsensitive, false);
                xml.WriteEndElement(); //options
            }

            if (_pageSize != null)
            {
                xml.WriteElement("pagesize", _pageSize, false);
            }

            if (_offset != null)
            {
                xml.WriteElement("offset", _offset, false);
            }
            
            xml.WriteEndElement(); //query

            xml.WriteEndElement(); //function
        }
    }
}