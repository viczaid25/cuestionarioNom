using AutoMapper;
using cuestionarioNom.Domain.Entities;
using cuestionarioNom.Domain.Enums;
using cuestionarioNom.Models.Dtos;
using cuestionarioNom.Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace cuestionarioNom.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Import DTO -> Entities
            CreateMap<ImportQuestionnaireDto, Questionnaire>();
            CreateMap<SectionDto, Section>();
            CreateMap<OptionDto, Option>();
            CreateMap<QuestionDto, Question>()
                .ForMember(d => d.Type, m => m.MapFrom(s =>
                    s.Type == "MultipleChoice" ? QuestionType.MultipleChoice :
                    s.Type == "Text" ? QuestionType.Text :
                    s.Type == "Number" ? QuestionType.Number :
                                                QuestionType.SingleChoiceLikert
                ));


            // Entities -> SurveyFillVm
            CreateMap<Questionnaire, SurveyFillVm>()
                .ForMember(d => d.QuestionnaireTitle, m => m.MapFrom(s => s.Title));
            CreateMap<Section, SectionVm>();
            CreateMap<Question, QuestionVm>();
            CreateMap<Option, OptionVm>();

            // Entities -> SurveyResultVm.ScoreVm
            CreateMap<Score, ScoreVm>()
                .ForMember(d => d.Scope, m => m.MapFrom(s => s.Scope.ToString()));
        }
    }
}
