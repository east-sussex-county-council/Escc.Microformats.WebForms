using System;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Escc.Dates;

namespace Escc.Microformats.WebForms
{
    /// <summary>
    /// Utility methods to support the HCalendar microformat
    /// </summary>
    public static class HCalendar
    {

        /// <summary>
        /// Gets the house-style description of a period from one date and time to another, with HTML controls which implement the hCalendar microformat
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="startTimeKnown">if set to <c>true</c> the supplied start time is correct. If <c>false</c>, 00.00 is assumed.</param>
        /// <param name="endTimeKnown">if set to <c>true</c> the supplied end time  is correct. If <c>false</c> 00.00 the next morning is assumed.</param>
        /// <param name="showStartTime">if set to <c>true</c> display the start time.</param>
        /// <param name="showEndTime">if set to <c>true</c> display the end time.</param>
        /// <returns>HTML controls with the date embedded</returns>
        public static Control[] DateRangeHCalendar(DateTime startDate, DateTime endDate, bool startTimeKnown, bool endTimeKnown, bool showStartTime, bool showEndTime)
        {
            bool multiDay = (startDate.DayOfYear != endDate.DayOfYear || startDate.Year != endDate.Year);
            bool showTime = (showStartTime || showEndTime);
            bool sameMonth = (startDate.Month == endDate.Month && startDate.Year == endDate.Year);

            // The display date may need to be different to the metadata (actual) date, so work with two copies
            DateTime displayStartDate = startDate;
            DateTime displayEndDate = endDate;

            if (!startTimeKnown) startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            if (!startTimeKnown || !showStartTime) displayStartDate = new DateTime(displayStartDate.Year, displayStartDate.Month, displayStartDate.Day);

            // all-day event goes up to midnight at the start of the NEXT day
            if (!endTimeKnown) endDate = new DateTime(endDate.AddDays(1).Year, endDate.AddDays(1).Month, endDate.AddDays(1).Day);
            if (!endTimeKnown || !showEndTime) displayEndDate = new DateTime(displayEndDate.Year, displayEndDate.Month, displayEndDate.Day);

            HtmlGenericControl tagStart = new HtmlGenericControl("time");
            tagStart.Attributes["class"] = "dtstart"; // hCalendar
            tagStart.Attributes["datetime"] = startDate.ToIso8601DateTime();  // hCalendar

            HtmlGenericControl tagEnd = new HtmlGenericControl("time");
            tagEnd.Attributes["class"] = "dtend"; // hCalendar
            tagEnd.Attributes["datetime"] = endDate.ToIso8601DateTime();  // hCalendar

            if (!multiDay && !showTime)
            {
                /*
                    One day, no time
                    ---------------------------------------
                    Friday 26 May 2006
                    */

                Control[] hCal = new Control[1];
                PlaceHolder ph = new PlaceHolder();

                hCal[0] = ph;
                ph.Controls.Add(tagStart);
                ph.Controls.Add(tagEnd);

                tagEnd.InnerText = displayStartDate.ToBritishDateWithDay();

                return hCal;

            }
            else if (!multiDay && ((showStartTime && !showEndTime) || (showStartTime && showEndTime && displayStartDate == displayEndDate)))
            {
                /*
                    One day, with start time (or matching start and end times)
                    ---------------------------------------
                    9am, Friday 26 May 2006
                    */

                Control[] hCal = new Control[1];
                PlaceHolder ph = new PlaceHolder();

                hCal[0] = ph;
                ph.Controls.Add(tagStart);
                ph.Controls.Add(tagEnd);

                tagEnd.InnerText = displayStartDate.ToBritishDateWithDayAndTime();

                return hCal;

            }
            else if (!multiDay && showStartTime && showEndTime)
            {
                /*
                    One day, with start and finish times
                    ---------------------------------------
                    9am to 2pm, Friday 26 May 2006
                    */

                Control[] hCal = new Control[1];
                PlaceHolder ph = new PlaceHolder();

                hCal[0] = ph;

                tagStart.InnerText = displayStartDate.ToBritishTime();
                ph.Controls.Add(tagStart);

                ph.Controls.Add(new LiteralControl(" to "));

                tagEnd.InnerText = (new StringBuilder(displayEndDate.ToBritishTime()).Append(", ").Append(displayStartDate.ToBritishDateWithDay()).ToString());
                ph.Controls.Add(tagEnd);

                return hCal;

            }
            else if (multiDay && !showTime && sameMonth)
            {
                /*
                    Different days, no times, same month
                    ---------------------------------------
                    26 to 27 May 2006
                    */

                Control[] hCal = new Control[1];
                PlaceHolder ph = new PlaceHolder();

                hCal[0] = ph;

                tagStart.InnerText = displayStartDate.Day.ToString(CultureInfo.CurrentCulture);
                ph.Controls.Add(tagStart);

                ph.Controls.Add(new LiteralControl(" to "));

                tagEnd.InnerText = displayEndDate.ToBritishDate();
                ph.Controls.Add(tagEnd);

                return hCal;

            }
            else if (multiDay && !showTime && !sameMonth)
            {
                /*
                    Different days, no times, different month
                    ---------------------------------------
                    Friday 26 May 2006 to Thursday 1 June 2006 
                    */

                Control[] hCal = new Control[1];
                PlaceHolder ph = new PlaceHolder();

                hCal[0] = ph;

                tagStart.InnerText = displayStartDate.ToBritishDateWithDay();
                ph.Controls.Add(tagStart);

                // Previous version using hyphen (for reference): ph.Controls.Add(new LiteralControl(" &#8211; "));
                ph.Controls.Add(new LiteralControl(" to "));

                tagEnd.InnerText = displayEndDate.ToBritishDateWithDay();
                ph.Controls.Add(tagEnd);

                return hCal;

            }
            else if (multiDay && showStartTime && !showEndTime)
            {
                /*
                    Different days, with start time
                    ---------------------------------------
                    Start: 9am, Friday 26 May 2006
                    Finish: Saturday 27 May 2006
                    */

                Control[] hCal = new Control[2];

                hCal[0] = tagStart;
                hCal[1] = tagEnd;

                tagStart.InnerText = displayStartDate.ToBritishDateWithDayAndTime();
                tagEnd.InnerText = displayEndDate.ToBritishDateWithDay();

                return hCal;

            }
            else if (multiDay && showStartTime && showEndTime)
            {
                /*
                    Different days, with start and end time
                    ---------------------------------------
                    Start: 9am, Friday 26 May 2006
                    Finish: 2pm, Saturday 27 May 2006
                    */

                Control[] hCal = new Control[2];

                hCal[0] = tagStart;
                hCal[1] = tagEnd;

                tagStart.InnerText = displayStartDate.ToBritishDateWithDayAndTime();
                tagEnd.InnerText = displayEndDate.ToBritishDateWithDayAndTime();

                return hCal;

            }

            // Shouldn't get here
            return new Control[0];
        }
    }
}
