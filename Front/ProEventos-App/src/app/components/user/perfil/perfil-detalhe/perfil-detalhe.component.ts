import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AbstractControlOptions, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ValidatorField } from '@app/helpers/ValidatorField';
import { UserUpdate } from '@app/models/identity/UserUpdate';
import { Palestrante } from '@app/models/Palestrante';
import { AccountService } from '@app/services/account.service';
import { PalestranteService } from '@app/services/palestrante.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-perfil-detalhe',
  templateUrl: './perfil-detalhe.component.html',
  styleUrls: ['./perfil-detalhe.component.scss']
})
export class PerfilDetalheComponent implements OnInit {

  @Output() changeFormValue = new EventEmitter();

  userUpdate = {} as UserUpdate;
  form!: FormGroup;

  public isAtivaSpinnerButton = false;

  get f(): any {
    return this.form.controls;
  }

  constructor(private fb: FormBuilder,
    public accountService: AccountService,
    public palestranteService: PalestranteService,
    private router: Router,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService) { }

  ngOnInit(): void {
    this.validation();
    this.carregarUsuario();
    this.verificaForm();
  }

  private verificaForm(): void{
    this.form.valueChanges.subscribe(() => this.changeFormValue.emit({ ... this.form.value }))
  }

  private carregarUsuario(): void{
    this.spinner.show();
    this.accountService.getUser().subscribe({
      next: (userResponse: UserUpdate) => {
        console.log(userResponse);
        this.userUpdate = userResponse;
        this.form.patchValue(this.userUpdate);
        this.toastr.success('Usuário carregado', 'Sucesso');
      },
      error: (error: any) => {
        console.error(error);
        this.toastr.error('Usuário não carregado', 'Erro');
      },
      complete: () => {
        this.spinner.hide();
      }
    })
  }

  private validation(): void{

    const formOptions: AbstractControlOptions = {
      validators: ValidatorField.MustMatch('password', 'confirmarPassword')
    }

    this.form = this.fb.group({
      userName: [''],
      titulo: ['NaoInformado', Validators.required],
      primeiroNome: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      ultimoNome: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', Validators.required],
      funcao: ['NaoInformado', Validators.required],
      descricao: ['', Validators.required],
      imagemURL: [''],
      password: ['', [Validators.nullValidator, Validators.minLength(6)]],
      confirmarPassword: ['', Validators.nullValidator]
    }, formOptions);

  }

  public resetForm(event: any): void{
    event.preventDefault();
    this.form.reset();
  }

  onSubmit(): void{
    this.atualizarUsuario();
  }

  public atualizarUsuario(){
    this.userUpdate = { ... this.form.value }
    this.spinner.show();
    this.isAtivaSpinnerButton = true;

    if(this.f.funcao.value == 'Palestrante'){
      this.palestranteService.post().subscribe({
        next: () => {
          this.toastr.success('Função palestrante ativada!', 'Sucesso');
        },
        error: (error: any) => {
          console.error(error);
          this.toastr.error('Função palestrante não pode ser ativada', 'Erro');
        }
      })
    }

    this.accountService.updateUser(this.userUpdate).subscribe({
      next: () => {
        this.toastr.success('Usuário atualizado', 'Sucesso');
      },
      error: (error: any) => {
        console.error(error);
        this.toastr.error('Não foi possível atualizar o usuário', 'Erro');
      },
      complete: () => {
        this.spinner.hide();
        this.isAtivaSpinnerButton = false;
      }
    });
  }

}
