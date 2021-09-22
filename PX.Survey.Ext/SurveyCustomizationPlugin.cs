using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Customization;

namespace PX.Survey.Ext
{
    public partial class SurveyCustomizationPlugin : CustomizationPlugin
    {

        public override void UpdateDatabase()
        {
            //todo: determine if this is needed may only need OnPublish. Purge when determined not needed 
        }

        //todo: follow up to add the auto code gen portion of this section
        public override void OnPublished()
        {
            InitializeSurveyComponents();
        }


        public static void InitializeSurveyComponent(SurveyComponent surveyComponent)
        {
            //todo: follow up and implement
            throw new NotImplementedException();
        }
    }
}
