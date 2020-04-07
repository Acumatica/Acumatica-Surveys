using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Covid19.Lib.DAC;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.SM;

namespace Covid19.Lib
{
    public class SurveyQuizSetting : PXGraph<SurveyQuizSetting, SurveyClass>
    {
        public SelectFrom<SurveyClass>.View SurveyClassCurrent;
        public CSAttributeGroupList<SurveyClass.surveyClassID, SurveyQuiz> Mapping;

        public SelectFrom<SurveyUser>.
            InnerJoin<EPEmployee>.On<SurveyUser.userid.IsEqual<EPEmployee.userID>>.View QuizUsers;

        public PXSetup<CRSetup> SurveySetup;
       
        public SurveyQuizSetting()
        {
            CRSetup Data = SurveySetup.Current;
        }

        public PXAction<SurveyClass> PrepopulateUsers;
        [PXButton]
        [PXUIField(DisplayName = "Prepopulate Users", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable prepopulateUsers(PXAdapter adapter)
        {
            var users = SelectFrom<Contact>.Where<Contact.contactType.IsEqual<ContactTypesAttribute.employee>.And<Contact.isActive.IsEqual<True>>>.View
                .Select(this);

            foreach (var user in users)
            {
                var quizUser = new SurveyUser();
                quizUser.SurveyClassID = SurveyClassCurrent.Current.SurveyClassID;
                quizUser.Userid = user.GetItem<Contact>().UserID;
                quizUser = QuizUsers.Insert(quizUser);
            }
            
            return adapter.Get();
        }
    }
}
