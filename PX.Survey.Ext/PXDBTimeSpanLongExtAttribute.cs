using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
// ReSharper disable IdentifierTypo

namespace PX.Survey.Ext
{
    public class PXDBTimeSpanLongExtAttribute : PXDBTimeSpanLongAttribute
    {
        //public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        //{
        //    try

        //    {
        //        if (string.IsNullOrEmpty(InputMask)) InputMask = "### d\\ays ## hrs ## mins";
        //        base.FieldSelecting(sender, e);
        //    }
        //    catch (Exception ex)
        //    {
        //        PXTrace.WriteError(ex);
        //        throw;
        //    }
        //}

        public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            if (string.IsNullOrEmpty(InputMask)) InputMask = "### d\\ays ## hrs ## mins";
            if (_AttributeLevel == PXAttributeLevel.Item || e.IsAltered)
            {
                string inputMask = _inputMasks[(int)this._Format];
                int lenght = _lengths[(int)this._Format];
                inputMask = PXMessages.LocalizeNoPrefix(inputMask);
                e.ReturnState = PXStringState.CreateInstance(e.ReturnState, lenght, null, _FieldName, _IsKey, null, String.IsNullOrEmpty(inputMask) ? null : inputMask, null, null, null, null);
            }

            int maskLenght = 0;

            foreach (char c in InputMask)
            {
                if (c == '#' || c == '0')
                    maskLenght += 1;
            }

            if (e.ReturnValue != null)
            {
                int mins = 0;
                int.TryParse(e.ReturnValue.ToString(), out mins);
                //var test = (int) e.ReturnValue;
                TimeSpan span = new TimeSpan(0, 0, mins, 0);
                int hours = (this._Format == TimeSpanFormatType.LongHoursMinutes) ? span.Days * 24 + span.Hours : span.Hours;
                var returnValue = string.Format(_outputFormats[(int)this._Format], span.Days, hours, span.Minutes);
                e.ReturnValue = returnValue.Length < 0 ? (new String(' ', maskLenght - returnValue.Length)) + returnValue : returnValue;
            }
        }

    }
}
