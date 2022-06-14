using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProEventos.Application.Dtos
{
    public class EventoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string Local { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string DataEvento { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório."),
        //MinLength(3, ErrorMessage = "{0} deve ter no mínimo 4 caracteres."),
        //MaxLength(50, ErrorMessage = "{0} deve ter no mínimo 50 caracteres.")]
        StringLength(50, MinimumLength = 3, ErrorMessage = "{0} deve ter entre 4 e 50 caracteres")]
        public string Tema { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório."),
        Display(Name = "Qtd Pessoas"),
        Range(1, 120000, ErrorMessage = "{0} não pode ser menor que 1 e maior que 120.000")]
        public int QtdPessoas { get; set; }

        [RegularExpression(@".*\.(gif|jpe?g|bmp|png)$", ErrorMessage = "Não é uma {0} válida (gif, jpg, jpeg, bmp ou png)."),
        Display(Name = "imagem")]
        public string ImagemURL { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório."),
        Phone(ErrorMessage = "O {0} não é válido.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório."),
        Display(Name = "e-mail"),
        EmailAddress(ErrorMessage = "O {0} não é válido.")]
        public string Email { get; set; }

        public int UserId { get; set; }
        public UserDto UserDto { get; set; }

        public IEnumerable<LoteDto> Lotes { get; set; }
        public IEnumerable<RedeSocialDto> RedesSociais { get; set; }
        public IEnumerable<PalestranteDto> Palestrante { get; set; }
    }
}