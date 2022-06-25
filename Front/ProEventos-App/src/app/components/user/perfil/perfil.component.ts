import { Component, OnInit } from '@angular/core';
import { UserUpdate } from '@app/models/identity/UserUpdate';
import { AccountService } from '@app/services/account.service';
import { environment } from '@environments/environment';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.scss']
})
export class PerfilComponent implements OnInit {

  public usuario = {} as UserUpdate;
  public imagemURL = '';
  public file: File;

  public get isPalestrante(): boolean {
    return this.usuario.funcao == 'Palestrante';
  }

  constructor(private spinner: NgxSpinnerService,
              private toastr: ToastrService,
              private acountService: AccountService)
              { }

  ngOnInit(): void {
  }

  public setFormValue(usuario: UserUpdate): void{
    this.usuario = usuario;
    if(this.usuario.imagemURL)
      this.imagemURL = environment.apiUrl + `resources/perfil/${this.usuario.imagemURL}`;
    else
      this.imagemURL = './assets/img/perfil.jpg';
  }

  public onFileChange(ev: any): void{
    const reader = new FileReader();

    reader.onload = (event: any) => this.imagemURL = event.target.result;

    this.file = ev.target.files;
    reader.readAsDataURL(this.file[0]);

    this.uploadImagem();
  }

  private uploadImagem(): void {
    this.spinner.show();
    this.acountService.postUpload(this.file).subscribe({
      next: () => {
        this.toastr.success('Imagem atualizada com sucesso!', 'Sucesso');
      },
      error: (error: any) => {
        console.error(error);
        this.spinner.hide();
        this.toastr.error('Erro ao fazer upload de imagem!', 'Erro');
      },
      complete: () => this.spinner.hide()
    });
  }

}
