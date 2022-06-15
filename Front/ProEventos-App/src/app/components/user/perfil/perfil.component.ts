import { Component, OnInit } from '@angular/core';
import { AbstractControlOptions, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ValidatorField } from '@app/helpers/ValidatorField';
import { UserUpdate } from '@app/models/identity/UserUpdate';
import { AccountService } from '@app/services/account.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.scss']
})
export class PerfilComponent implements OnInit {

  userUpdate = {} as UserUpdate;
  form!: FormGroup;

  get f(): any {
    return this.form.controls;
  }

  constructor(private fb: FormBuilder,
              public accountService: AccountService,
              private router: Router,
              private toastr: ToastrService,
              private spinner: NgxSpinnerService) { }

  ngOnInit() {
    this.validation();
    this.carregarUsuario();
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
      }
    });
  }

}
