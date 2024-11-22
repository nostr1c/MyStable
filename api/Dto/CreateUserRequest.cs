using FluentValidation;
using api.Services;


namespace api.Dto
{
    public class CreateUserRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Biography { get; set; }
        public string Avatar { get; set; }
    }

    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        private readonly UserRepository _userRepository;
        private readonly ILogger<CreateUserRequestValidator> _logger;

        public CreateUserRequestValidator(UserRepository repository, ILogger<CreateUserRequestValidator> logger)
        {
            _userRepository = repository;
            _logger = logger;

            RuleFor(x => x.Firstname)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("ErrorFirstnameCannotBeEmpty")
            .Length(1, 50).WithMessage("ErrorFirstnameLengthRange");

            RuleFor(x => x.Lastname)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ErrorLastnameCannotBeEmpty")
                .Length(1, 50).WithMessage("ErrorLastnameLengthRange");

            RuleFor(x => x.Username)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ErrorUsernameCannotBeEmpty")
                .Length(3, 50).WithMessage("ErrorUsernameLengthRange")
                .MustAsync(async (username, cancellation) =>
                {
                    bool exists = await _userRepository.UsernameAlreadyExists(username);

                    _logger.LogInformation($"Username check: {username} exists: {exists}");

                    return !exists;
                }).WithMessage("ErrorUsernameAlreadyExist");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("ErrorEmailCannotBeEmpty")
                .Matches(@"^((?!\.)[\w\-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$")
                    .WithMessage("ErrorEmailFormat")
                .MustAsync(async (email, cancellation) =>
                {
                    bool exists = await _userRepository.EmailAlreadyExists(email);

                    _logger.LogInformation($"Email check: {email} exists: {exists}");

                    return !exists;
                }).WithMessage("ErrorEmailAlreadyExist");
                
            RuleFor(x => x.Biography)
                .MaximumLength(255).WithMessage("ErrorBiographyLengthRange");

            RuleFor(x => x.Avatar)
                .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
                .WithMessage("ErrorAvatarUrlFormat")
                .When(x => !string.IsNullOrWhiteSpace(x.Avatar));
        }
    }
}
