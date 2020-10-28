using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PX.Common;
using PX.Data;
using PX.Data.Access.ActiveDirectory;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.SM;
using PX.Survey.Ext;

namespace AcumaticaSurveysLibr
{
    [PXCacheName("Filter users roles")]

    public class SurveyMaint : PXGraph<SurveyMaint, Survey>
    {
        public SelectFrom<Survey>.View SurveyCurrent;

        public PXFilter<FilterUserRoles> FilterRoles;
        public PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<EPEmployee.bAccountID>>>> CurrentEmployee;

        public PXSelect<UsersInRoles, Where<UsersInRoles.applicationName, Equal<Current<PX.SM.Roles.applicationName>>,
            And<UsersInRoles.rolename, Equal<Current<PX.SM.Roles.rolename>>>>> UsersByRole;

        [PXViewName(PX.Objects.CR.Messages.Attributes)]
        public CSAttributeGroupList<Survey.surveyID, SurveyCollector> Mapping;

        public SelectFrom<SurveyUser>.Where<SurveyUser.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.View SurveyUsers;

        [PXHidden] [PXCopyPasteHiddenView] public PXSetup<SurveySetup> SurveySetup;

        [PXCopyPasteHiddenView]
        public SelectFrom<Contact>.
            Where<Contact.contactType.IsEqual<ContactTypesAttribute.employee>.
                And<Contact.isActive.IsEqual<True>>.
                And<Contact.userID.IsNotNull>>.OrderBy<Asc<Contact.displayName>>.View UsersForAddition;

        [PXHidden]
        [PXCopyPasteHiddenView]
        public SelectFrom<SurveyCollector>.Where<SurveyCollector.surveyID.IsEqual<Survey.surveyID.FromCurrent>>.View
            SurveyCollector;

        public SurveyMaint()
        {
            SurveySetup Data = SurveySetup.Current;

            UsersForAddition.Cache.AllowInsert = false;
            UsersForAddition.Cache.AllowDelete = false;
        }

        public PXAction<Survey> AddUsers;

        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(VisibleOnDataSource = false)]
        public virtual IEnumerable addUsers(PXAdapter adapter)
        {
            var users = UsersForAddition.Select().Where(a => a.GetItem<Contact>().Selected == true).ToList();

            foreach (var user in users)
            {
                var surveyUser = new SurveyUser();
                surveyUser.Active = true;
                surveyUser.SurveyID = SurveyCurrent.Current.SurveyID;
                surveyUser.ContactID = user.GetItem<Contact>().ContactID;
                surveyUser = SurveyUsers.Insert(surveyUser);
            }

            return adapter.Get();
        }

        public PXAction<Survey> AddRecipients;

        [PXButton]
        [PXUIField(DisplayName = "Add Recipients", MapViewRights = PXCacheRights.Select,
            MapEnableRights = PXCacheRights.Select)]
        public virtual IEnumerable addRecipients(PXAdapter adapter)
        {
            if (UsersForAddition.AskExt((graph, viewName) =>
            {
                graph.Views[UsersForAddition.View.Name].Cache.Clear();
                graph.Views[viewName].Cache.Clear();
                graph.Views[viewName].Cache.ClearQueryCache();
                graph.Views[viewName].ClearDialog();
            }, true) != WebDialogResult.OK) return adapter.Get();

            return addUsers(adapter);
        }

        protected virtual void _(Events.RowSelecting<Survey> e)
        {
            Survey row = e.Row;
            if (row == null)
            {
                return;
            }

            using (new PXConnectionScope())
            {
                var collectorData = SelectFrom<SurveyCollector>.Where<SurveyCollector.surveyID.IsEqual<@P.AsInt>>.View
                    .SelectWindowed(this, 0, 1, row.SurveyID).TopFirst;
                row.IsSurveyInUse = (collectorData != null);
            }
        }

        protected virtual void _(Events.RowSelected<Survey> e)
        {
            Survey currentSurvey = e.Row;
            if (currentSurvey == null)
            {
                return;
            }

            bool unlockSurvey = !(currentSurvey.IsSurveyInUse.GetValueOrDefault(false));

            e.Cache.AllowDelete = unlockSurvey;
            Mapping.Cache.AllowUpdate = unlockSurvey;
            Mapping.Cache.AllowInsert = unlockSurvey;
            Mapping.Cache.AllowDelete = unlockSurvey;
            PXUIFieldAttribute.SetEnabled<Survey.surveyName>(e.Cache, currentSurvey, unlockSurvey);
        }

        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXFormula(typeof(MobileAppDeviceOS<Contact.userID>))]
        [PXDependsOnFields(typeof(Contact.contactID), typeof(Contact.userID))]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), "Visibility", PXUIVisibility.SelectorVisible)]
        protected virtual void Contact_UsrMobileAppDeviceOS_CacheAttached(PXCache sender)
        {
        }

        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXFormula(typeof(IIf<Where<ContactSurveyExt.usrMobileAppDeviceOS, IsNull>, False, True>))]
        [PXDependsOnFields(typeof(ContactSurveyExt.usrMobileAppDeviceOS))]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), "Visibility", PXUIVisibility.SelectorVisible)]
        protected virtual void Contact_UsrUsingMobileApp_CacheAttached(PXCache sender)
        {
        }

        [PXMergeAttributes]
        [PXParent(typeof(Select<Survey, Where<Survey.surveyID, Equal<Current<CSAttributeGroup.entityClassID>>>>),
            LeaveChildren = true)]
        [PXDBLiteDefault(typeof(Survey.surveyIDStringID))]
        protected virtual void _(Events.CacheAttached<CSAttributeGroup.entityClassID> e)
        {
        }

        public class SelectorCustomerContractAttribute : PXCustomSelectorAttribute
        {
            private Type selectorField;
            private Type contractFld;

            public SelectorCustomerContractAttribute(Type selectorField, Type contractField)
                : base(typeof(Roles.rolename))
            {
                if (selectorField == null)
                    throw new ArgumentNullException("selectorField");

                if (contractField == null)
                    throw new ArgumentNullException("contractField");


                if (BqlCommand.GetItemType(selectorField).Name != BqlCommand.GetItemType(selectorField).Name)
                {
                    throw new ArgumentException(string.Format(
                        "moduleField and docTypeField must be of the same declaring type. {0} vs {1}",
                        BqlCommand.GetItemType(selectorField).Name, BqlCommand.GetItemType(selectorField).Name));
                }

                this.selectorField = selectorField;
                contractFld = contractField;
            }

            public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
            {
            }

            protected virtual IEnumerable GetRecords()
            {
                var cache = this._Graph.Caches[BqlCommand.GetItemType(selectorField)];
                var cbs = (Contact)cache.Current;
                cache = this._Graph.Caches[BqlCommand.GetItemType(contractFld)];
                var contract = (Roles)cache.Current;
                var result = new List<int>();
                return result;
            }
        }

        protected virtual IEnumerable usersForAddition()
        {
            var greedLineStartQuery =
                new PXSelectJoin<Contact, InnerJoin<EPEmployee, On<EPEmployee.userID, Equal<Contact.userID>>>>(this);

            var summaryCurrent = FilterRoles.Current;
            if (summaryCurrent.DepartmentID != null)
            {
                greedLineStartQuery.WhereAnd<Where<EPEmployee.departmentID, Equal<Current<FilterUserRoles.departmentID>>>>();
            }
            if (summaryCurrent.VendorClassID != null)
            {
                greedLineStartQuery.WhereAnd<Where<EPEmployee.vendorClassID, Equal<Current<FilterUserRoles.vendorClassID>>>>();
            }
            if (summaryCurrent.ParentBAccountID != null)
            {
                greedLineStartQuery.WhereAnd<Where<EPEmployee.parentBAccountID, Equal<Current<FilterUserRoles.parentBAccountID>>>>();
            }

            var lines = greedLineStartQuery.Select().ToList().Select(a => a.GetItem<Contact>()).ToList().Distinct();
            return lines;
        }
    }
}
