using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.ElephantParade.Core.Interfaces;
using NHSD.ElephantParade.Core.Mapping;
using NHSD.ElephantParade.Core.Models;
using NHSD.ElephantParade.DAL;
using NHSD.ElephantParade.DAL.Interfaces;
using NHSD.ElephantParade.DAL.Repositories;
using NHSD.ElephantParade.DAL.EntityModels;
using System.Web;
using System.Globalization;

namespace NHSD.ElephantParade.Core
{
    public class CallbackService : ICallbackService
    {
        enum CallEvents
        {
            RecordLocked,
            RecordUnLocked,

        }

        #region fields and constructors
        //Member variables
        private ICallbackRepository _callbackRepository;
        private ICallbackCommentsRepository _callbackCommentsRepository;
        private IStudiesService _studiesService;
        private INonSecureEmailService _emailService;

        //Constructors
        public CallbackService(IStudiesService studiesService, INonSecureEmailService emailService)
            : this(new CallbackRepository(), studiesService, emailService, new CallbackCommentsRepository())
        {
        }

        public CallbackService(ICallbackRepository callbackRepository, 
            IStudiesService studiesService,
            INonSecureEmailService emailService,
            ICallbackCommentsRepository patientCommentsRepository)
        {
            _callbackRepository = callbackRepository ?? new CallbackRepository();
            _callbackCommentsRepository = patientCommentsRepository ?? new CallbackCommentsRepository();
            _studiesService = studiesService;
            _emailService = emailService;
        }
        #endregion

        public void CallBack_SendAppoinmentNotificationEmail(CallbackViewModel callbackVM)
        { 
            string CallBackDate = callbackVM.CallbackDate.HasValue ? callbackVM.CallbackDate.Value.ToString("dd/MM/yyyy") : string.Empty;
            DateTime time = DateTime.Today.Add(callbackVM.CallbackStartTime);
            string CallBackTime = time.ToString("hh:mm tt"); // It will give - e.g. "3:00 PM"

            //send email for any callback happening today, tomorrow, or next day.
            //the sql job is expected to run in the early hours of tomorrow morning, any scheduled for
            //today + 4 days will be sent an email then.
            if (callbackVM.CallbackDate < DateTime.Now.Date.AddDays(4))
            {
                string textBody = "Dear " + callbackVM.Patient.Forename + ",<p>Your next telephone appointment with the Healthlines Service is booked as shown below.</p>" + "<p>" +
                    "A Healthlines Advisor will call you at " + CallBackTime + " on " + CallBackDate + ".</p>" +
                    "<p>Please do not reply to this email, as this mailbox is not monitored. If you need to contact us before your appointment, please email us at healthlines.nhsd@nhs.net or call 0345 603 0897." + "</p>" +
                    "<p>Kind regards</p>" +
                    "<p>Healthlines Service Team</p>" +
                    "<p>Please be advised that the information sent in this e-mail by the Healthlines Service is only for the person named in the message. If you are not this person, or you are not expecting information from us, please disregard and delete the message." +
                    "<p>The content of this message is provided for information purposes only, and is not intended as a substitute for a consultation with a health professional.</p>" +
                    "<p>If you have further queries connected with this information, please contact us on 0345 603 0897 or email us at: healthlines.nhsd@nhs.net and a Healthlines Advisor will get back to you between the hours of 10.00-20.00 hrs weekdays and 10.00-14.00 hrs on Saturdays.</p>";

                _emailService.SendEmail(callbackVM.Patient.Email,
                    "Healthlines Service – Appointment Reminder",
                    textBody,
                    null, false);
            }
        }

        /// <summary>
        /// Mark any open callbacks as complete
        /// </summary>
        /// <param name="newCallbackVM"></param>
        public void CallBack_AddAndCompletePrevious(CallbackViewModel newCallbackVM, Guid currentCallbackID, string currentCallOutcome)
        {
            //MarkAllPatientCallbacksAsCompleted(callbackVM.Patient.PatientId, callbackVM.Patient.StudyID, callOutcome);
            //CallBack_Add(callbackVM);

            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            var callback = cvmo.ConvertFromCallbackViewModel(newCallbackVM);
            var cbq = _callbackRepository.GetQueryable();
                        
            string patientId = callback.PatientId;
            string studyId = callback.StudyID;
            var originalCallback = (from cb in cbq
                            where cb.PatientId == patientId &&
                                  cb.StudyID == studyId &&
                                    cb.CallbackId == currentCallbackID
                            select cb).FirstOrDefault();
            
            if (originalCallback == null)
                throw new ArgumentNullException(string.Format("currentCallbackID {0} cannot be found", currentCallbackID.ToString()));

            originalCallback.Completed = true;
            originalCallback.CallOutcome = currentCallOutcome;

            CallBackAdd(callback, currentCallOutcome, currentCallbackID);
            CallBack_SendAppoinmentNotificationEmail(newCallbackVM);

            _callbackRepository.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callbackVM"></param>
        public void CallBack_Add(CallbackViewModel callbackVM, String calloutcome = Constants.EncounterIncompleteText, bool sendNotification = true)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            var callback = cvmo.ConvertFromCallbackViewModel(callbackVM);

            CallBackAdd(callback, calloutcome, Guid.Empty);

            _callbackRepository.SaveChanges();
            callbackVM.CallbackId = callback.CallbackId;

            if (sendNotification)
            {
                CallBack_SendAppoinmentNotificationEmail(callbackVM);
            }
        }

        /// <summary>
        /// adds a callback object to the repsoitory and marks old callbacks as rescheduled
        /// </summary>
        /// <param name="callback"></param>
        private void CallBackAdd(Callback callback, String outcome, Guid filterId)
        {
            //update older callback records
            string patientId = callback.PatientId;
            string studyId = callback.StudyID;
            var cbq = _callbackRepository.GetQueryable();

            //if its a patient callback request always add without rescheduling other callbacks
            if (callback.Type != (int)CallbackViewModel.CallbackType.Patient)
            {
                List<Callback> callbacks;
                if (filterId == Guid.Empty)
                {
                    callbacks = (from cb in cbq
                                 where cb.PatientId == patientId &&
                                       cb.StudyID == studyId &&
                                       !cb.Completed && //completed callbacks do not need to be mark as rescheduled!
                                       cb.Type != (int) CallbackViewModel.CallbackType.Recheduled
                                 //don't need to mark callbacks that are already rescheduled.
                                 select cb).ToList();
                }
                else
                {
                    callbacks = (from cb in cbq
                                 where cb.PatientId == patientId &&
                                       cb.StudyID == studyId &&
                                       !cb.Completed && //completed callbacks do not need to be mark as rescheduled!
                                       cb.Type != (int) CallbackViewModel.CallbackType.Recheduled &&
                                       cb.CallbackId != filterId
                                 //don't need to mark callbacks that are already rescheduled.
                                 select cb).ToList();
                }
                foreach (var item in callbacks.ToList())
                {
                    item.Type = (int) CallbackViewModel.CallbackType.Recheduled;
                    item.CallOutcome = outcome;
                }
            }

            //add the callback
            _callbackRepository.Add(callback);
        }
        /// <summary>
        /// deletes a callback entry
        /// </summary>
        /// <param name="guid"></param>
        public void CallBack_Delete(Guid guid)
        {
            var callback = _callbackRepository.GetSingle(c => c.CallbackId == guid);
            if (callback != null)
            {
                _callbackRepository.Delete(callback);
                _callbackRepository.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CallbackId"></param>
        /// <param name="userID"></param>
        public void LockCallbackRecord(Guid CallbackId, string userID)
        {
            var callbackObj = _callbackRepository.GetSingle(c => c.CallbackId == CallbackId);

            //lock the record in patient table
            callbackObj.LockedDate = DateTime.Now;
            callbackObj.LockedTo = userID.ToLower(); 

            //Add history event
            CallEvent ce = new CallEvent();
            ce.EventCode = CallEvents.RecordLocked.ToString();
            ce.Message = string.Empty;
            ce.Date = callbackObj.LockedDate.Value;
            ce.CallbackID = CallbackId;
            ce.UserID = userID.ToLower();
            ce.PatientId = callbackObj.PatientId;
            ce.StudyID = callbackObj.StudyID;
            _callbackRepository.CallEvent_Add(ce);

            _callbackRepository.SaveChanges();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CallbackId"></param>
        /// <param name="userID"></param>
        public void UnLockCallbackRecord(Guid CallbackId, string userID)
        {
            var callbackObj = _callbackRepository.GetSingle(c => c.CallbackId == CallbackId);
            //lock the record in patient table
            callbackObj.LockedDate = null;
            callbackObj.LockedTo = null;

            //Add history event
            CallEvent ce = new CallEvent();
            ce.EventCode = CallEvents.RecordUnLocked.ToString();
            ce.Message = string.Empty;
            ce.Date = DateTime.Now;
            ce.CallbackID = CallbackId;
            ce.UserID = userID;
            ce.PatientId = callbackObj.PatientId;
            ce.StudyID = callbackObj.StudyID;
            _callbackRepository.CallEvent_Add(ce);

            _callbackRepository.SaveChanges();
        }



        /// <summary>
        /// ToDo:// handel studyID
        /// </summary>
        /// <param name="patientID"></param>
        /// <param name="studyID"></param>
        public void MarkPatientCallbacksWithPreviousCallsAsCompleted(string patientID, string studyID)
        {
            var cbq = _callbackRepository.GetQueryable();

            //variables needed to compare within linq statement below
            DateTime currentDate = DateTime.Now.Date;
            DateTime currentDateAndTime = DateTime.Now;

            //Guid CadsPatientID = Guid.Empty;
            //if (Guid.TryParse(patientID, out CadsPatientID))
            //    throw new ArgumentException("not guid");

            var callbacks = from cb in cbq
                            where cb.PatientId == patientID &&
                            cb.StudyID == studyID &&
                            !cb.Completed && //no point marking completed callbacks as completed again
                            (cb.CallbackDate < currentDate ||
                            (cb.CallbackDate == currentDate &&
                            cb.CallbackStartTime < currentDateAndTime)) //ensure we dont mark as completed any callbacks which occur in the future
                            select cb;

            foreach (var callback in callbacks.ToList())
            {
                callback.Completed = true;
            }

            _callbackRepository.SaveChanges();
        }

        /// <summary>
        /// Retrieve from the database, a list of new patients (and asssociated patients), that need to be actioned.
        /// </summary>
        /// <returns></returns>
        public PaginatedViewModel<CallbackViewModel> Callbacks_ListLockedToUser(int page, int records)
        {
            return GetCallbacksLockedToUserList(null, null, page, records);
        }

        /// <summary>
        /// Overloaded method to allow retrieval of a single patient's callbacks.
        /// </summary>
        public IList<CallbackViewModel> Callbacks_ListLockedToUser(string patientId, string studyId)
        {
            //TODO: refactor this to use PaginatedViewModel
            return GetCallbacksLockedToUserList(patientId, studyId).Data;
        }

        public PaginatedViewModel<CallbackViewModel> GetCallbacksLockedToUserList(string patientId, string studyId, int page = 0, int records = 10)
        {
            var queryable = _callbackRepository.GetQueryable();
            var inactivePatients = _studiesService.GetPatientsInactive();
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            PaginatedViewModel<CallbackViewModel> paginatedVM = new PaginatedViewModel<CallbackViewModel>();

            var callbacks = (from c in queryable
                             where c.LockedDate != null &&
                                   !c.Completed &&
                                   c.LockedTo == HttpContext.Current.User.Identity.Name && 
                                   !inactivePatients.Contains(c.PatientId) &&
                                   c.Type != (int)CallbackViewModel.CallbackType.Recheduled
                             select new
                             {
                                 Item = c,
                                 Count = queryable.Where(cb => cb.PatientId == c.PatientId && cb.StudyID == c.StudyID).Count()
                             }).ToList();

            var lockedPatientsCallbacks = callbacks.OrderBy(x => x.Item.CallbackDate).ThenBy(x => x.Item.CallbackStartTime).Skip(page * records).Take(records).ToList();

            var result = (from c in lockedPatientsCallbacks
                          select cvmo.ConvertFromCallback(c.Item, _studiesService, c.Count)).ToList();

            //remove null patients before returning
            //quick fix - would prefer to do this in linq above, but there is no relationship in database between callback and patient table.
            IList<CallbackViewModel> nullPatientCallbacks = result.Where(callback => callback.Patient == null).ToList();

            foreach (var callback in nullPatientCallbacks)
            {
                result.Remove(callback);
            }

            paginatedVM.TotalRecordsCount = callbacks.Count();
            paginatedVM.Data = result.ToList();

            return paginatedVM;
        }


        /// <summary>
        /// Retrieve from the database, a list of new patients (and asssociated patients), that need to be actioned.
        /// </summary>
        /// <returns></returns>
        public PaginatedViewModel<CallbackViewModel> Callbacks_List_PatientsNotCalled(int page, int records)
        {
            return GetPatientsNotCalledList(null, null, page, records);
        }

        /// <summary>
        /// Overloaded method to allow retrieval of a single patient's callbacks.
        /// </summary>
        public IList<CallbackViewModel> Callbacks_List_PatientsNotCalled(string patientId, string studyId)
        {
            //TODO: refactor this to use PaginatedViewModel
            return GetPatientsNotCalledList(patientId, studyId).Data;
        }

        public PaginatedViewModel<CallbackViewModel> GetPatientsNotCalledList(string patientId, string studyId, int page = 0, int records = 10)
        {
            var queryable = _callbackRepository.GetQueryable();
            var inactivePatients = _studiesService.GetPatientsInactive();
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            PaginatedViewModel<CallbackViewModel> paginatedVM = new PaginatedViewModel<CallbackViewModel>();

            var callbacks = (from c in queryable
                                 where c.LockedDate == null &&
                                       !c.Completed &&
                                       !inactivePatients.Contains(c.PatientId) &&
                                       c.Type != (int)CallbackViewModel.CallbackType.Recheduled
                                 select new
                                            {
                                                Item = c,
                                                Count = queryable.Where(cb => cb.PatientId == c.PatientId && cb.StudyID == c.StudyID).Count()
                                            }).ToList();

            //select patients who have only callback in the system
            var patientsNotCalled = (from c in callbacks
                                        where c.Count == 1
                                        select c).ToList();

            paginatedVM.TotalRecordsCount = patientsNotCalled.Count();

            var newPatientsCallbacks = patientsNotCalled.OrderBy(x => x.Item.CallbackDate).ThenBy(x => x.Item.CallbackStartTime).Skip(page * records).Take(records).ToList();

            var result = (from c in newPatientsCallbacks
                            select cvmo.ConvertFromCallback(c.Item, _studiesService, c.Count)).ToList();

            //remove null patients before returning
            //quick fix - would prefer to do this in linq above, but there is no relationship in database between callback and patient table.
            IList<CallbackViewModel> nullPatientCallbacks = result.Where(callback => callback.Patient == null).ToList();

            foreach (var callback in nullPatientCallbacks)
            {
                result.Remove(callback);
            }

            paginatedVM.Data = result.ToList();

            return paginatedVM;
        }

        /// <summary>
        /// Retrieve from the database, a list of scheduled callbacks (and asssociated patients), that need to be actioned.
        /// </summary>
        /// <returns></returns>
        public PaginatedViewModel<CallbackViewModel> Callbacks_List(int page, int records)
        {
            return GetScheduledCallbacks(null, null, page, records);
        }

        /// <summary>
        /// Retrieve from the database, a list of scheduled callbacks (and asssociated patients), that need to be actioned.
        /// </summary>
        /// <returns></returns>
        public PaginatedViewModel<CallbackViewModel> Callbacks_List(string patientId, string studyId, int page, int records)
        {
            return GetScheduledCallbacks(patientId, studyId, page, records);
        }

        /// <summary>
        /// Overloaded method to allow retrieval of a single patient's callbacks.
        /// </summary>
        public IList<CallbackViewModel> Callbacks_List(string patientId, string studyId)
        {
            //TODO: refactor this to use PaginatedViewModel
            return GetScheduledCallbacks(patientId, studyId).Data;
        }

        private PaginatedViewModel<CallbackViewModel> GetScheduledCallbacks(string patientId, string studyId, int page = 0, int records = 10)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            PaginatedViewModel<CallbackViewModel> paginatedVM = new PaginatedViewModel<CallbackViewModel>();

            var inactivePatients = _studiesService.GetPatientsInactive();

            var queryable = _callbackRepository.GetQueryable();

            //are we searching for a specific patient, or for all patients
            if (!string.IsNullOrEmpty(patientId) && !string.IsNullOrEmpty(studyId))
            {
                var callbacks = (from c in queryable
                                 where c.LockedDate == null && !c.Completed && c.Type != (int)CallbackViewModel.CallbackType.Recheduled
                                 && c.PatientId == patientId && c.StudyID == studyId
                                 && !inactivePatients.Contains(c.PatientId)
                                 select new
                                 {
                                     Count = queryable.Where(cb => cb.Completed && cb.PatientId == c.PatientId && cb.StudyID == c.StudyID).Count(),
                                     Item = c
                                 }).ToList();

                paginatedVM.Data = (from c in callbacks select cvmo.ConvertFromCallback(c.Item, _studiesService, c.Count)).OrderBy(x => x.CallbackDate).ThenBy(x => x.CallbackStartTime).ToList();
                
                //TODO:need to add count of all records, and paging to this part.
                paginatedVM.TotalRecordsCount = callbacks.Count();

                return paginatedVM;
            }
            else
            {
                //get all patient callbacks - so use paging

                var callbacksQuery = (from c in queryable
                                 where c.LockedDate == null && !c.Completed && c.Type != (int)CallbackViewModel.CallbackType.Recheduled
                                 && !inactivePatients.Contains(c.PatientId)
                                 select new
                                 {
                                     Count = queryable.Where(cb => cb.Completed && cb.PatientId == c.PatientId && cb.StudyID == c.StudyID).Count(),
                                     Item = c
                                 });

                paginatedVM.TotalRecordsCount = callbacksQuery.Count();

                var callbacks = callbacksQuery.OrderBy(x => x.Item.CallbackDate).ThenBy(x => x.Item.CallbackStartTime).Skip(page * records).Take(records).ToList();

                var result = (from c in callbacks 
                              select cvmo.ConvertFromCallback(c.Item, _studiesService, c.Count)).ToList();
                
                //remove null patients before returning
                //quick fix - would prefer to do this in linq above, but there is no relationship in database between callback and patient table.
                IList<CallbackViewModel> nullPatientCallbacks = result.Where(callback => callback.Patient == null).ToList();

                foreach (var callback in nullPatientCallbacks)
                {
                    result.Remove(callback);
                }

                paginatedVM.Data = result.ToList();

                return paginatedVM;
            }
        }

        /// <summary>
        /// Retrieve from the database, a list of new patients (and asssociated patients), that need to be actioned.
        /// </summary>
        /// <returns></returns>
        public PaginatedViewModel<CallbackViewModel> Callbacks_List_Locked(int page, int records)
        {
            return GetCallbacksLockedList(null, null, page, records);
        }

        /// <summary>
        /// Overloaded method to allow retrieval of a single patient's callbacks.
        /// </summary>
        public IList<CallbackViewModel> Callbacks_List_Locked(string patientId, string studyId)
        {
            //TODO: refactor this to use PaginatedViewModel
            return GetCallbacksLockedList(patientId, studyId).Data;
        }

        public PaginatedViewModel<CallbackViewModel> GetCallbacksLockedList(string patientId, string studyId, int page = 0, int records = 10)
        {
            var queryable = _callbackRepository.GetQueryable();
            var inactivePatients = _studiesService.GetPatientsInactive();
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            PaginatedViewModel<CallbackViewModel> paginatedVM = new PaginatedViewModel<CallbackViewModel>();

            var callbacks = (from c in queryable
                             where c.LockedDate != null &&
                                   !c.Completed &&
                                   !inactivePatients.Contains(c.PatientId) &&
                                   c.Type != (int)CallbackViewModel.CallbackType.Recheduled
                             select new
                             {
                                 Item = c,
                                 Count = queryable.Where(cb => cb.PatientId == c.PatientId && cb.StudyID == c.StudyID).Count()
                             }).ToList();

            var lockedPatientsCallbacks = callbacks.OrderBy(x => x.Item.CallbackDate).ThenBy(x => x.Item.CallbackStartTime).Skip(page * records).Take(records).ToList();

            var result = (from c in lockedPatientsCallbacks
                          select cvmo.ConvertFromCallback(c.Item, _studiesService, c.Count)).ToList();

            //remove null patients before returning
            //quick fix - would prefer to do this in linq above, but there is no relationship in database between callback and patient table.
            IList<CallbackViewModel> nullPatientCallbacks = result.Where(callback => callback.Patient == null).ToList();

            foreach (var callback in nullPatientCallbacks)
            {
                result.Remove(callback);
            }

            paginatedVM.TotalRecordsCount = callbacks.Count();
            paginatedVM.Data = result.ToList();

            return paginatedVM;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="callbackID"></param>
        /// <returns></returns>
        public CallbackViewModel Callbacks_Get(Guid callbackID)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            //var scheduledCallbacks = _callbackRepository.GetAll(cb => cb.CallbackId == callbackID).ToList();
            var queryable = _callbackRepository.GetQueryable();
            var callbacks = queryable.Where(c => c.CallbackId == callbackID 
                && c.Completed == false
                && c.Type != (int)CallbackViewModel.CallbackType.Recheduled).AsQueryable().Select(c =>
                        new
                        {
                            Count = queryable.Where(cb => cb.Completed && cb.PatientId == c.PatientId
                                && cb.StudyID == c.StudyID).Count(),
                            Item = c
                        }).ToList();
            if (callbacks.Count > 0)
                return cvmo.ConvertFromCallback(callbacks[0].Item, _studiesService, callbacks[0].Count);
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public CallbackViewModel Callbacks_GetAnyType(Guid callbackID)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();

            var queryable = _callbackRepository.GetQueryable();
            var callbacks = queryable.Where(c => c.CallbackId == callbackID).AsQueryable().Select(c =>
                        new
                        {
                            Count = queryable.Where(cb => cb.Completed && cb.PatientId == c.PatientId
                                && cb.StudyID == c.StudyID).Count(),
                            Item = c
                        }).ToList();
            if (callbacks.Count > 0)
                return cvmo.ConvertFromCallback(callbacks[0].Item, _studiesService, callbacks[0].Count);
            else
                return null;
        }

        /// <summary>
        /// ToDo: record and return events
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="studyID"></param>
        /// <returns></returns>
        public IList<CallEventViewModel> CallEvent_ListForPatient(string patientId, string studyID)
        {      
            //ready a return list
            List<CallEventViewModel> returnList = new List<CallEventViewModel>();

            //Get User Comments
            var comments = _callbackCommentsRepository.GetQueryable().Where(c => c.PatientId == patientId && c.StudyID == studyID).ToList();

            var cvmo = new ConvertViewModelObjects();
            returnList.AddRange(comments.Select(i => cvmo.ConvertFromPatientComments(i)));

            var events = _callbackRepository.CallEvent_List().Where(ce => ce.PatientId == patientId && ce.StudyID == studyID).ToList();
            returnList.AddRange(events.Select(i => cvmo.ConvertFromCallEvent(i)));

            return returnList;
        }

        /// <summary>
        /// Record and return events
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="studyID"></param>
        /// <returns></returns>
        public IList<CallEventViewModel> CallEvent_ListForPatientAndCall(string patientId, string studyID, Guid callbackID)
        {
            //ready a return list
            List<CallEventViewModel> returnList = new List<CallEventViewModel>();

            var cvmo = new ConvertViewModelObjects();
            var events = _callbackRepository.CallEvent_List().Where(ce => ce.CallbackID == callbackID).ToList();
            returnList.AddRange(events.Select(i => cvmo.ConvertFromCallEvent(i)));

            return returnList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callbackID"></param>
        /// <returns></returns>
        public IList<CallEventViewModel> CallEvent_List(Guid callbackID)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            return _callbackRepository.CallEvent_List().Where(ce => ce.CallbackID == callbackID).Select(ce=>cvmo.ConvertFromCallEvent(ce)).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callEventViewModel"></param>
        public void CallEvent_Add(CallEventViewModel callEventViewModel)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            var ce = cvmo.ConvertFromCallEventViewModel(callEventViewModel);
            _callbackRepository.CallEvent_Add(ce);
            _callbackRepository.SaveChanges();
        }
        

        /// <summary>
        /// Get Patient comments order by Descending Date
        /// </summary>
        /// <param name="whereCondition"></param>
        /// <returns></returns>
        public IList<PatientCallCommentViewModel> PatientComments_List(string patientId, string studyID)
        {
            var patientCommentsHistory = _callbackCommentsRepository.GetAll(x => x.PatientId == patientId && x.StudyID == studyID).OrderByDescending(x => x.CommentsDate).ToList();
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            var patientListViewModel = (from patientComments in patientCommentsHistory
                                        select cvmo.ConvertFromPatientComments(patientComments)).ToList();
            return patientListViewModel.ToList();
        }


        public void PatientComments_Add(PatientCallCommentViewModel PatientCommentsVM)
        {
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            var pComments = cvmo.ConvertFromPatientCommentsViewModel(PatientCommentsVM);
            _callbackCommentsRepository.Add(pComments);
            _callbackCommentsRepository.SaveChanges();
            PatientCommentsVM.CommentId = pComments.CommentID;
        }


        public PatientCallCommentViewModel PatientComments_Get(int commentID)
        {
            var patientComment = _callbackCommentsRepository.GetSingle(x => x.CommentID == commentID);
            ConvertViewModelObjects cvmo = new ConvertViewModelObjects();
            var patientCommentViewModel = cvmo.ConvertFromPatientComments(patientComment);
            return patientCommentViewModel;
        }

        public void PatientComments_Delete(int commentID)
        {
            var patientComment = _callbackCommentsRepository.GetSingle(x => x.CommentID == commentID);
            if (patientComment != null)
            {
                _callbackCommentsRepository.Delete(patientComment);
                _callbackCommentsRepository.SaveChanges();
            }
        }

    }
}