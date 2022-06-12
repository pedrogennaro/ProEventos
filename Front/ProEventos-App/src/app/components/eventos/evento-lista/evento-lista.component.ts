import { Component, OnInit, TemplateRef } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Evento } from '@app/models/Evento';
import { EventoService } from '@app/services/evento.service';
import { DatePipe } from '@angular/common';
import { Constants } from '@app/util/constants';
import { environment } from '@environments/environment';

@Component({
  selector: 'app-evento-lista',
  templateUrl: './evento-lista.component.html',
  styleUrls: ['./evento-lista.component.scss']
})
export class EventoListaComponent implements OnInit {

  modalRef!: BsModalRef;

  public eventos: Evento[] = [];
  public eventosFiltrados: Evento[] = [];
  public eventoId = 0;

  public widthImg = 150;
  public marginImg = 2;
  public showImg = true;
  private filtroListado: string = '';

  public get filtroLista(){
    return this.filtroListado;
  }

  public set filtroLista(value: string){
    this.filtroListado = value;
    this.eventosFiltrados = this.filtroLista ? this.filtrarEventos(this.filtroLista) : this.eventos;
  }

  public filtrarEventos(filtrarPor: string): Evento[]{
    filtrarPor = filtrarPor.toLocaleLowerCase();
    return this.eventos.filter(
      (evento: any) => evento.tema.toLocaleLowerCase().indexOf(filtrarPor) !== -1 ||
      evento.local.toLocaleLowerCase().indexOf(filtrarPor) !== -1
    );
  }

  constructor(
      private eventoService: EventoService,
      private modalService: BsModalService,
      private toastr: ToastrService,
      private spinner: NgxSpinnerService
    ) { }

  public ngOnInit(): void {
    this.spinner.show();
    this.getEventos();
  }

  public hideImg(): void{
    this.showImg = !this.showImg;
  }

  public mostraImagem(imagemURL: string): string{
    return (imagemURL != '') ? `${environment.apiUrl}resources/images/${imagemURL}` : 'assets/img/semImagem.png';
  }

  public getEventos(): void {
    this.eventoService.getEvento().subscribe({
      next: (responseEventos: Evento[]) => {
        this.eventos = responseEventos;
        this.eventosFiltrados = this.eventos;
      },
      error: (error:any) => {
        this.spinner.hide(),
        this.toastr.error('Erro ao carregar os eventos', 'Erro');
      },
      complete: () => this.spinner.hide()
    })
    // this.eventoService.getEvento().subscribe(
    //   (responseEventos: Evento[]) => {
    //     this.eventos = responseEventos,
    //     this.eventosFiltrados = this.eventos
    //   },
    //   error => console.log(error)
    // )
  }

  openModal(template: TemplateRef<any>, eventoId: number): void {
    this.eventoId = eventoId;
    this.modalRef = this.modalService.show(template, {class: 'modal-sm'});
  }

  confirm(): void {
    this.modalRef.hide();
    this.spinner.show();

    this.eventoService.deleteEvento(this.eventoId).subscribe({
      next: (result: any) => {
        if(result.message === 'Deletado'){
          this.toastr.success('O evento foi deletado com sucesso.', 'Deletado');
          this.spinner.hide();
          this.getEventos();
        }
      },
      error: (error: any) => {
        console.error(error);
        this.toastr.error(`Erro ao tentar deletar o evento ${this.eventoId}`, 'Erro');
        this.spinner.hide();
      },
      complete: () => this.spinner.hide()
    });
  }

  decline(): void {
    this.modalRef.hide();
  }

  formatDate(campoData: any) {
    let arrData = campoData.split('/');
    let stringFormatada = arrData[1] + '-' + arrData[0] + '-' +arrData[2];

    const datepipe: DatePipe = new DatePipe('en-US')
    let formattedDate = datepipe.transform(stringFormatada, Constants.DATE_TIME_FMT);
    return formattedDate;
  }

}
