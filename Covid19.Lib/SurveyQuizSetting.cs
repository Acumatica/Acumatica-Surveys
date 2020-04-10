using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Covid19.Lib.DAC;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.SM;

namespace Covid19.Lib
{
    public class SurveyQuizSetting : PXGraph<SurveyQuizSetting, SurveyClass>
    {
        public SelectFrom<SurveyClass>.View SurveyClassCurrent;
        public CSAttributeGroupList<SurveyClass.surveyClassID, SurveyCollector> Mapping;

        public SelectFrom<SurveyUser>.Where<SurveyUser.surveyClassID.IsEqual<SurveyClass.surveyClassID.FromCurrent>>.View QuizUsers;

        [PXHidden]
        [PXCopyPasteHiddenView]
        public PXSetup<CRSetup> SurveySetup;

        [PXHidden]
        [PXCopyPasteHiddenView]
        public SelectFrom<Contact>.InnerJoin<EPEmployee>.On<Contact.userID.IsEqual<EPEmployee.userID>>.Where<Contact.contactType.IsEqual<ContactTypesAttribute.employee>.
            And<Contact.isActive.IsEqual<True>>.And<Contact.userID.IsNotNull>>.View UsersForAddition;

        [PXHidden]
        [PXCopyPasteHiddenView]
        public SelectFrom<SurveyCollector>.Where<SurveyCollector.surveyID.IsEqual<SurveyClass.surveyClassID.FromCurrent>>.View SurveyCollector;

       
        public SurveyQuizSetting()
        {
            CRSetup Data = SurveySetup.Current;

            UsersForAddition.Cache.AllowInsert = false;
            UsersForAddition.Cache.AllowDelete = false;
        }


        [PXUIField(DisplayName = "Collector ID")]
        [PXDBString(60, IsUnicode = true)]
        protected virtual void _(Events.CacheAttached<SurveyCollector.collectorName> e) { }

        public PXAction<SurveyClass> CreateSurvey;
        [PXButton]
        [PXUIField(DisplayName = "Create Survey", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable createSurvey(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, delegate ()
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
                collector.CollectorName = SurveyClassCurrent.Current.SurveyName + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                collector.SurveyID = SurveyClassCurrent.Current.SurveyClassID;
                collector.Userid = user.Userid;
                collector.CollectedDate = null;
                collector.ExpirationDate = null;
                collector.CollectorStatus = "N";
                collector = SurveyCollector.Update(collector);
            }

            this.Persist();
        }

        public PXAction<SurveyClass> ClearSurvey;

        [PXButton]
        [PXUIField(DisplayName = "Clear Survey",
                   MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable clearSurvey(PXAdapter adapter)
        {
            if (SurveyClassCurrent.Ask("Delete", "Are you sure?", "Are you sure you want to delete all new Surveys?",
                    MessageButtons.YesNo) != WebDialogResult.Yes) return adapter.Get();

            PXLongOperation.StartOperation(this, delegate ()
            {
                ClearSurveys();
            });
            return adapter.Get();
        }

        private void ClearSurveys()
        {
            var newSurveys = SurveyCollector.Select().Where(s => s.GetItem<SurveyCollector>().CollectorStatus == "N")
                .ToList();

            foreach (var newSurvey in newSurveys)
            {
                SurveyCollector.Delete(newSurvey);
            }

            Persist();
        }

        public PXAction<SurveyClass> addUsers;

        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable AddUsers(PXAdapter adapter)
        {
            var users = UsersForAddition.Select().Where(a => a.GetItem<Contact>().GetExtension<ContactSrvExt>().Add == true).ToList();
            var usersAlreadyInList = QuizUsers.Select().Select(a => a.GetItem<SurveyUser>().Userid).ToList();
            usersAlreadyInList.AddRange(QuizUsers.Cache.Inserted.ToArray<SurveyUser>().Select(a => a.Userid));
            

            //TODO @Dhiren replace with [PXCheckUnique(typeof(Where<SurveyUser.surveyClassID,
            //Equal<Current<SurveyUser.surveyClassID>>>), ClearOnDuplicate = false)]
            users = users.Where(u => !usersAlreadyInList.Contains(u.GetItem<Contact>().UserID)).ToList();

            foreach (var user in users)
            {
                var quizUser = new SurveyUser();
                quizUser.Active = true;
                quizUser.SurveyClassID = SurveyClassCurrent.Current.SurveyClassID;
                quizUser.Userid = user.GetItem<Contact>().UserID;
                quizUser = QuizUsers.Insert(quizUser);
            }

            return adapter.Get();
        }

        public PXAction<SurveyClass> PrepopulateUsers;

        [PXButton]
        [PXUIField(DisplayName = "Prepopulate Users", MapViewRights = PXCacheRights.Select,
            MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable prepopulateUsers(PXAdapter adapter)
        {
            if (UsersForAddition.AskExt((graph, viewName) =>
            {
                graph.Views[UsersForAddition.View.Name].Cache.Clear();
                graph.Views[viewName].Cache.Clear();
                graph.Views[viewName].Cache.ClearQueryCache();
                graph.Views[viewName].ClearDialog();
            }, true) != WebDialogResult.OK) return adapter.Get();

            return AddUsers(adapter);
        }

        protected virtual void _(Events.RowSelected<SurveyClass> e)
        {
            var currentSurvey = e.Row;
            if (currentSurvey == null)
                return;

            CreateSurvey.SetEnabled(currentSurvey.Active == true);

            bool inactiveOrNull = currentSurvey.Active != true;

            Mapping.AllowInsert = inactiveOrNull;
            Mapping.AllowUpdate = inactiveOrNull;
            Mapping.AllowDelete = inactiveOrNull;

            QuizUsers.AllowInsert = inactiveOrNull;
            QuizUsers.AllowUpdate = inactiveOrNull;
            QuizUsers.AllowDelete = inactiveOrNull;
        }
    }
}