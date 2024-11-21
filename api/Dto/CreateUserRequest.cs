using FluentValidation;

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
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Firstname)
                .NotEmpty()
                .WithMessage("Firstname cannot be empty");
            RuleFor(x => x.Lastname)
                .NotEmpty()
                .WithMessage("Lastname cannot be empty");
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username cannot be empty");
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email cannot be empty")
                .Matches(@"^((?!\.)[\w\-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$")
                .WithMessage("Email doesn't match an email format");
        }
    }
}
