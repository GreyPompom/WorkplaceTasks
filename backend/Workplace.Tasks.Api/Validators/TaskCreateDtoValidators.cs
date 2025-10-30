using FluentValidation;
using Workplace.Tasks.Api.DTOs;

namespace Workplace.Tasks.Api.Validators
{
    public class TaskCreateDtoValidator : AbstractValidator<TaskCreateDto>
    {
        public TaskCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("O título é obrigatório.")
                .MaximumLength(250).WithMessage("O título deve ter no máximo 250 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("A descrição deve ter no máximo 2000 caracteres.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("O status informado não é válido.");
        }
    }
}
