using api.Services;
using FluentValidation;

namespace api.Dto
{
    public class UpdateUserRequest
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Biography { get; set; }
        public string Avatar { get; set; }
    }

    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        private readonly UserRepository _userRepository;
        private readonly ILogger<UpdateUserRequestValidator> _logger;

        public UpdateUserRequestValidator(UserRepository userRepository, ILogger<UpdateUserRequestValidator> logger)
        {
            _userRepository = userRepository;
            _logger = logger;

            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .MustAsync(async (id, cancellation) =>
                {
                    var user = await _userRepository.GetUserByIdAsync(id);

                    return user != null;
                }).WithMessage("ErrorUserDoesNotExist");

            RuleFor(x => x.Firstname)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ErrorFirstnameCannotBeEmpty")
                .Length(1, 50).WithMessage("ErrorFirstnameLength");

            RuleFor(x => x.Lastname)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ErrorLastnameCannotBeEmpty")
                .Length(1, 50).WithMessage("ErrorLastnameLength");

            RuleFor(x => x.Biography)
                .MaximumLength(255).WithMessage("ErrorBiographyLength");

            RuleFor(x => x.Avatar)
                .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
                .WithMessage("ErrorAvatarUrlFormat")
                .When(x => !string.IsNullOrWhiteSpace(x.Avatar));
        }
    }
}
