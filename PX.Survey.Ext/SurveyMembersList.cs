using PX.Data;
using PX.Objects.CR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace PX.Survey.Ext {

    public class SurveyMembersList : CRMembersList<Survey, SurveyMember, OperationParam> {

        public SurveyMembersList(PXGraph graph) : base(graph) {
            View = new PXView(_Graph, false, new Select2<SurveyMember, LeftJoin<Contact, On<SurveyMember.contactID, Equal<Contact.contactID>, And<SurveyMember.surveyID, Equal<Current<Survey.surveyID>>>>, LeftJoin<Address, On<Address.addressID, Equal<Contact.defAddressID>>>>, Where2<Where<Contact.contactType, Equal<ContactTypesAttribute.lead>, Or<Contact.contactType, Equal<ContactTypesAttribute.person>, Or<Contact.contactType, Equal<ContactTypesAttribute.bAccountProperty>, Or<Contact.contactType, Equal<ContactTypesAttribute.employee>>>>>, And<SurveyMember.surveyID, Equal<Current<Survey.surveyID>>>>>());
        }

        public override IEnumerable MultipleDelete(PXAdapter adapter) {
            Survey current = base.primaryCache.Current as Survey;
            if (current == null) {
                return adapter.Get();
            }
            if (current == null || current.SurveyID == null || base.primaryCache.GetStatus(current) == PXEntryStatus.Inserted) {
                return adapter.Get();
            }
            List<SurveyMember> cRCampaignMembers = new List<SurveyMember>();
            PXCache item = _Graph.Caches[typeof(SurveyMember)];
            foreach (SurveyMember cRCampaignMember in ((IEnumerable<SurveyMember>)item.Updated).Concat<SurveyMember>((IEnumerable<SurveyMember>)item.Inserted)) {
                bool? selected = cRCampaignMember.Selected;
                if (!(selected.GetValueOrDefault() & selected.HasValue)) {
                    continue;
                }
                cRCampaignMembers.Add(cRCampaignMember);
            }
            if (!cRCampaignMembers.Any<SurveyMember>() && item.Current != null) {
                cRCampaignMembers.Add((SurveyMember)item.Current);
            }
            foreach (SurveyMember cRCampaignMember1 in cRCampaignMembers) {
                item.Delete(cRCampaignMember1);
            }
            return adapter.Get();
        }

        public override IEnumerable MultipleInsert(PXAdapter adapter) {
            Survey current = base.primaryCache.Current as Survey;
            if (current == null) {
                return adapter.Get();
            }
            if (current.SurveyID != null && base.primaryCache.GetStatus(current) != PXEntryStatus.Inserted) {
                base.filterCurrent.CampaignID = current.SurveyID;
                base.filterCurrent.Action = "Add";
                base.AskExt((PXGraph g, string name) => g.Caches[typeof(Contact)].Clear());
            }
            return adapter.Get();
        }

        protected override int ProcessList(List<Contact> list) {
            return PXProcessing.ProcessItems<Contact>(list, (Contact item) => {
                OrderedDictionary orderedDictionaries = new OrderedDictionary()
                {
                    { "CampaignID", base.filterCurrent.CampaignID },
                    { "ContactID", item.ContactID }
                };
                if (base.filterCurrent.Action == "Add") {
                    base.membersCache.Update(orderedDictionaries, orderedDictionaries);
                    return;
                }
                base.membersCache.Delete(orderedDictionaries, orderedDictionaries);
            });
        }

        protected override void TMain_RowSelected(PXCache cache, PXRowSelectedEventArgs e) {
            _Graph.Actions["DeleteAction"].SetEnabled(_Graph.Views["CampaignMembers"].SelectSingle(Array.Empty<object>()) != null);
        }
    }
}