using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using NHSD.ElephantParade.Core;
using NHSD.ElephantParade.Core.Interfaces;
using Autofac.Core;
using Questionnaires.Core.Services;
using Questionnaires.DAL.Repositories;
using Questionnaires.Core.DataAccess.Interfaces;
using NHSD.ElephantParade.DocumentGenerator;
using NHSD.ElephantParade.DocumentGenerator.Interfaces;
using System.Configuration;

namespace NHSD.ElephantParade.Web
{
    /// <summary>
    /// http://code.google.com/p/autofac/wiki/StructuringWithModules
    /// </summary>
    public class PatientCallbackModule :Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            #region Questionnaire Mappings
            builder.Register(c => new QuestionnaireRepository()).As<IQuestionnaireRepository>();
            builder.Register(c => new QuestionRepository()).As<IQuestionRepository>();
            builder.Register(c => new QuestionSetRepository()).As<IQuestionSetRepository>();
            builder.Register(c => new QuestionSetQuestionRepository()).As<IQuestionSetQuestionRepository>();

            builder.Register(c => new AnswerOptionRepository()).As<IAnswerOptionRepository>();
            builder.Register(c => new AnswerSetRepository()).As<IAnswerSetRepository>();
            builder.RegisterType<QuestionnaireService>().As<IQuestionnaireService>();
            #endregion

            #region Callback Study Mappings
            //example
            //CADS Patient Maps
            builder.Register(c => new DepressionService(c.Resolve<IQuestionnaireService>())).As<IDepressionService>();

            builder.Register(c => new StudyService(c.Resolve<IDepressionService>(), null, null, null)).As<IStudiesService>();


            //builder.RegisterType<PatientService>().Named<IPatientService>("DepressionPatients");
            //builder.RegisterType<NHSD.ElephantParade.Duke.PatientService>().Named<IPatientService>("CvdPatients");
            //builder.Register(c => new StudyService(c.ResolveNamed<IPatientService>("DepressionPatients"),
            //                                        c.ResolveNamed<IPatientService>("CvdPatients"))
            //                ).As< IStudiesService>();            

            //replaced by the above as the named params are resolved as singeltons
            //List<NamedParameter> prams = new List<NamedParameter>();            
            //prams.Add(new NamedParameter("DepressionPatients", new PatientService(new ElephantParade.DAL.Repositories.PatientRepository(),new ElephantParade.DAL.Repositories.PatientMedicationRepository())));
            //prams.Add(new NamedParameter("CvdPatients", new NHSD.ElephantParade.Duke.PatientService()));
            //builder.RegisterType<StudyService>().As<IStudiesService>().WithParameters(prams).InstancePerLifetimeScope();

            builder.RegisterType<ElephantParade.DAL.Repositories.PatientMedicationRepository>().As<ElephantParade.DAL.Interfaces.IPatientMedicationRepository>();

            builder.RegisterType<ContentSectionStatusService>().As<IContentSectionStatusService>();
            builder.RegisterType<CallbackService>().As<ICallbackService>();
            builder.RegisterType<ReadingService>().As<IReadingService>();
            builder.RegisterType<PageVisitedLogService>().As<IPageVisitedLogService>();
            #endregion

            #region email

            builder.Register(c => new EmailService(ConfigurationManager.AppSettings["NetSMTPHost"],
                                                    int.Parse(ConfigurationManager.AppSettings["NetSMTPPort"]),
                                                    ConfigurationManager.AppSettings["NetAccountUserName"],
                                                    ConfigurationManager.AppSettings["NetAccountPassword"],
                                                    ConfigurationManager.AppSettings["NetEmailAddress"])).As<IEmailService>();

            builder.Register(c => new NonSecureEmailService(ConfigurationManager.AppSettings["NonSecureSMTPHost"],
                                        int.Parse(ConfigurationManager.AppSettings["NonSecureSMTPPort"]),
                                        ConfigurationManager.AppSettings["NonSecureAccountUserName"],
                                        ConfigurationManager.AppSettings["NonSecureAccountPassword"],
                                        ConfigurationManager.AppSettings["NonSecureEmailAddress"])).As<INonSecureEmailService>();

            #endregion

            #region DocumentGeneration Mapping

            builder.Register(c => new DocumentService()).As<IDocumentService>();
            
            #endregion

            #region PasswordResetRequest Mapping

            builder.Register(c => new PasswordResetRequestService()).As<IPasswordResetRequestService>();

            #endregion

            #region Membership Mapping

            builder.Register(c => new MembershipService(c.Resolve<INonSecureEmailService>(), c.Resolve<IStudiesService>())).As<IMembershipService>();

            #endregion
        }
    }
}