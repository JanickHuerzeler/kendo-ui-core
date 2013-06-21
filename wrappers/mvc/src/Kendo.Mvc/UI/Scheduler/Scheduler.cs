﻿namespace Kendo.Mvc.UI
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Kendo.Mvc.Infrastructure;
    using System.IO;
using System;

    /// <summary>
    /// The server side wrapper for Kendo UI Scheduler
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class Scheduler<TModel> : WidgetBase, IScheduler<TModel> 
        where TModel : class, ISchedulerEvent
    {
        public Scheduler(ViewContext viewContext,
                    IJavaScriptInitializer initializer,
                    IUrlGenerator urlGenerator
            )
            : base(viewContext, initializer)
        {
            DataSource = new DataSource();

            DataSource.Type = DataSourceType.Ajax;

            DataSource.ModelType(typeof(TModel));

            UrlGenerator = urlGenerator;

            Resources = new List<SchedulerResource<TModel>>();

            Views = new List<SchedulerViewBase>();
        }

        public DataSource DataSource
        {
            get;
            private set;
        }

        public IUrlGenerator UrlGenerator
        {
            get;
            private set;
        }

        public DateTime? Date
        {
            get;
            set;
        }

        public DateTime? StartTime
        {
            get;
            set;
        }

        public DateTime? EndTime
        {
            get;
            set;
        }

        public int? Height
        {
            get;
            set;
        }

        public string EventTemplate
        {
            get;
            set;
        }

        public string EventTemplateId
        {
            get;
            set;
        }

        public SchedulerEditableSettings Editable
        {
            get;
            set;
        }

        public IList<SchedulerResource<TModel>> Resources
        {
            get;
            private set;
        }

        public IList<SchedulerViewBase> Views
        {
            get;
            private set;
        }

        public override void WriteInitializationScript(TextWriter writer)
        {
            var options = this.SeriailzeBaseOptions();

            writer.Write(Initializer.Initialize(Selector, "Scheduler", options));

            base.WriteInitializationScript(writer);
        }

        protected virtual IDictionary<string, object> SeriailzeBaseOptions()
        {
            var options = new Dictionary<string, object>(Events);

            var idPrefix = "#";

            if (Date != null)
            {
                options["date"] = Date;
            }

            if (StartTime != null)
            {
                options["startTime"] = StartTime;
            }

            if (EndTime != null)
            {
                options["endTime"] = EndTime;
            }

            if (Height != null)
            {
                options["height"] = Height;
            }

            if (Editable != null)
            {
                if (Editable.Enable == false)
                {
                    options["editable"] = false;
                }
                else if (Editable.ToJson().Count > 0)
                {
                    options["editable"] = Editable.ToJson();
                }
            }

            if (!string.IsNullOrEmpty(EventTemplate))
            {
                options["eventTemplate"] = EventTemplate;
            }

            if (!string.IsNullOrEmpty(EventTemplateId))
            {
                options["eventTemplate"] = new ClientHandlerDescriptor { HandlerName = String.Format("kendo.template($('{0}{1}').html())", idPrefix, EventTemplateId) };      
            }

            if (Resources.Count > 0)
            {
                options["resources"] = Resources.ToJson();
            }

            if (Views.Count > 0)
            {
                options["views"] = Views.ToJson();
            }

            Dictionary<string, object> dataSource = (Dictionary<string, object>)DataSource.ToJson();

            dataSource["type"] = "scheduler-aspnetmvc";

            //TODO: update logic
            if (DataSource.Data != null)
            {
                dataSource["data"] = new { Data = DataSource.Data };
            }

            options["dataSource"] = dataSource;

            return options;
        }

        protected override void WriteHtml(System.Web.UI.HtmlTextWriter writer)
        {
            SchedulerHtmlBuilder<TModel> builder = new SchedulerHtmlBuilder<TModel>(this);

            builder.Build().WriteTo(writer);

            base.WriteHtml(writer);
        }
    }
}
