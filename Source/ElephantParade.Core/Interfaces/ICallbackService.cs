using System.Collections.Generic;
using NHSD.ElephantParade.Core.Models;
using System;

namespace NHSD.ElephantParade.Core.Interfaces
{
    public interface ICallbackService //: IService<CallbackViewModel>
    {
        //void MarkPatientCallbacksWithPreviousCallsAsRescheduled(string patientID, string studyID);

        //void MarkPatientCallbacksWithPreviousCallsAsCompleted(string patientID, string studyID);
        //void MarkAllPatientCallbacksAsCompleted(string patientID, string studyID);
        #region Callbacks
        void CallBack_Add(CallbackViewModel callbackVM,String outcome = Constants.EncounterIncompleteText, bool sendNotification = true);
        void CallBack_AddAndCompletePrevious(CallbackViewModel newCallbackVM, Guid currentCallbackID,string currentCallOutcome);
        void LockCallbackRecord(Guid CallbackId, string userID);
        void UnLockCallbackRecord(Guid CallbackId, string userID);
        //IList<CallbackViewModel> CallbackLocked_ListByUser(string LockedTo);
        //IList<CallbackViewModel> CallbackLocked_List();
        //IList<CallbackViewModel> Callbacks_ListLocked();

        PaginatedViewModel<CallbackViewModel> Callbacks_ListLockedToUser(int page = 0, int records = 10);
        IList<CallbackViewModel> Callbacks_ListLockedToUser(string patientId, string studyId);
        PaginatedViewModel<CallbackViewModel> Callbacks_List(string patientId, string studyId, int page = 0, int records = 10);
        PaginatedViewModel<CallbackViewModel> Callbacks_List(int page = 0, int records = 10);
        IList<CallbackViewModel> Callbacks_List(string patientId, string studyId);
        PaginatedViewModel<CallbackViewModel> Callbacks_List_PatientsNotCalled(int page = 0, int records = 10);
        IList<CallbackViewModel> Callbacks_List_PatientsNotCalled(string patientId, string studyId);
        PaginatedViewModel<CallbackViewModel> Callbacks_List_Locked(int page = 0, int records = 10);
        IList<CallbackViewModel> Callbacks_List_Locked(string patientId, string studyId);

        CallbackViewModel Callbacks_Get(Guid callbackID);
        CallbackViewModel Callbacks_GetAnyType(Guid callbackID);

        /// <summary>
        /// Deletes a callback entry
        /// </summary>
        /// <param name="guid"></param>
        void CallBack_Delete(Guid guid);
        #endregion

        #region Events
        IList<CallEventViewModel> CallEvent_ListForPatient(string patientId, string studyID);
        IList<CallEventViewModel> CallEvent_ListForPatientAndCall(string patientID, string studyID, Guid callbackID);
        IList<CallEventViewModel> CallEvent_List(Guid callbackID);
        void CallEvent_Add(CallEventViewModel patientHistoryViewModel);
        #endregion

        #region Callback comments
        PatientCallCommentViewModel PatientComments_Get(int commentID);
        IList<PatientCallCommentViewModel> PatientComments_List(string patientId, string studyID);
        void PatientComments_Add(PatientCallCommentViewModel PatientCommentsVM);
        void PatientComments_Delete(int commentID);        
        #endregion





        
    }
}