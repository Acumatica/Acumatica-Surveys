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
            InnerJoin<EPEmployee>.On<SurveyUser.userid.IsEqual<EPEmployee.userID>>.Where<SurveyUser.surveyClassID.IsEqual<SurveyClass.surveyClassID.FromCurrent>>.View QuizUsers;

        public PXSetup<CRSetup> SurveySetup;
        
        [PXHidden]
        public SelectFrom<SurveyCollector>.Where<SurveyCollector.surveyID.IsEqual<SurveyClass.surveyClassID.FromCurrent>>.View SurveyCollector;

       
        public SurveyQuizSetting()
        {
            CRSetup Data = SurveySetup.Current;
        }

        public PXAction<SurveyClass> CreateSurvey;
        [PXButton]
        [PXUIField(DisplayName = "Create Survey", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable createSurvey(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, delegate()
            {
                CreateSurveys();
            });

            return adapter.Get();
        }

        private void CreateSurveys()
        {
            var activeUsers = QuizUsers.Select().Where(a => a.GetItem<SurveyUser>().Active == true).ToList();
            var alreadyInserted = SurveyCollector.Select().ToList().Select(a => a.GetItem<SurveyCollector>().Userid).ToList();
            var allUsers = activeUsers.Select(a => a.GetItem<SurveyUser>().Userid).ToList();
            var idsForInsertion = allUsers.Except(alreadyInserted);

            activeUsers = activeUsers.Where(u => idsForInsertion.Contains(u.GetItem<SurveyUser>().Userid)).ToList();

            foreach (PXResult<SurveyUser> activeUser in activeUsers)
            {
                var user = activeUser.GetItem<SurveyUser>();
                var collector = SurveyCollector.Insert(new SurveyCollector());
                collector.SurveyID = SurveyClassCurrent.Current.SurveyClassID;
                collector.Userid = user.Userid;
                collector.CollectedDate = null;
                collector.ExpirationDate = null;
                collector.CollectorStatus = "N";
                collector = SurveyCollector.Update(collector);
            }

            this.Persist();
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
