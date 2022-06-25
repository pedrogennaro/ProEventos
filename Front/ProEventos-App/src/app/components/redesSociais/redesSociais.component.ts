import { Component, Input, OnInit, TemplateRef } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { RedeSocial } from '@app/models/RedeSocial';
import { RedeSocialService } from '@app/services/redeSocial.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-redesSociais',
  templateUrl: './redesSociais.component.html',
  styleUrls: ['./redesSociais.component.scss']
})
export class RedesSociaisComponent implements OnInit {

  modalRef: BsModalRef;
  @Input() eventoId = 0;
  public formRS: FormGroup;
  public redeSocialAtual = { id: 0, nome: '', indice: 0 }

  get redesSociais(): FormArray {
    return this.formRS.get('redesSociais') as FormArray;
  }

  constructor(private fb: FormBuilder,
              private modalService: BsModalService,
              private spinner: NgxSpinnerService,
              private toastr: ToastrService,
              private redeSocialService: RedeSocialService)
              { }

  ngOnInit() {
    this.carregarRedesSociais(this.eventoId);
    this.validation();
  }

  public carregarRedesSociais(id: number = 0): void{
    let origem = 'palestrante';

    if(this.eventoId != 0) origem = 'evento';

    this.spinner.show();
    this.redeSocialService.getRedesSociais(origem, id).subscribe({
      next: (redeSocialRetorno: RedeSocial[]) => {
        redeSocialRetorno.forEach((redeSocial) => {
          this.redesSociais.push(this.criarRedeSocial(redeSocial));
        });
      },
      error: (error: any) => {
        console.error(error);
        this.toastr.error('Erro ao tentar carregar rede social!', 'Erro');
      },
      complete: () => this.spinner.hide()
    })
  }

  public validation(): void{
    this.formRS = this.fb.group({
      redesSociais: this.fb.array([])
    })
  }

  adicionarRedeSocial(): void{
    this.redesSociais.push(this.criarRedeSocial({id: 0} as RedeSocial));
  }

  criarRedeSocial(redeSocial: RedeSocial): FormGroup {
    return this.fb.group({
      id: [redeSocial.id],
      nome: [redeSocial.nome, Validators.required],
      url: [redeSocial.url, Validators.required],
    });
  }

  public retornaTitulo(valor: string): string{
    return valor === null || valor === ''
    ? 'Nome da Rede Social'
    : valor
  }

  public cssValidator(campoForm: FormControl | AbstractControl): any{
    return {'is-invalid': campoForm.errors && campoForm.touched };
  }

  public salvarRedesSociais(): void{
    let origem = 'palestrante';

    if(this.eventoId != 0) origem = 'evento';

    if(this.formRS.controls['redesSociais'].valid){
      this.spinner.show();
      this.redeSocialService.saveRedesSociais(origem, this.eventoId, this.formRS.value.redesSociais).subscribe({
        next: () => {
          this.toastr.success('Redes Sociais salvas com sucesso!', 'Sucesso')
        },
        error: (error: any) => {
          console.error(error);
          this.toastr.error('Erro ao salvar rede social!', 'Erro');
        },
        complete: () => this.spinner.hide()
      });
    }
  }

  public removerRedeSocial(template: TemplateRef<any>, indice: number): void{
    this.redeSocialAtual.id = this.redesSociais.get(indice + '.id').value;
    this.redeSocialAtual.nome = this.redesSociais.get(indice + '.nome').value;
    this.redeSocialAtual.indice = indice;
    this.modalRef = this.modalService.show(template, {class: 'modal-sm'});
  }

  public confirmDeleteRedeSocial(): void{
    this.modalRef.hide();
    this.spinner.show();

    let origem = 'palestrante';

    if(this.eventoId != 0) origem = 'evento';

    this.redeSocialService.deleteRedesSociais(origem, this.eventoId, this.redeSocialAtual.id).subscribe({
      next: () => {
        this.toastr.success('Rede Social deletado com sucesso!', 'Sucesso');
        this.redesSociais.removeAt(this.redeSocialAtual.indice);
      },
      error: (error: any) => {
        console.error(error);
        this.spinner.hide();
        this.toastr.error('Erro ao deletar rede social!', 'Erro');
      },
      complete: () => this.spinner.hide()
    });
  }

  public declineDeleteRedeSocial(): void{
    this.modalRef.hide();
  }

}
