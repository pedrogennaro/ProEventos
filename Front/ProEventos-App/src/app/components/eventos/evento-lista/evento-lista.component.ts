import { Component, OnInit, TemplateRef } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Evento } from '@app/models/Evento';
import { EventoService } from '@app/services/evento.service';
import { DatePipe } from '@angular/common';
import { Constants } from '@app/util/constants';
import { environment } from '@environments/environment';
import { PaginatedResult, Pagination } from '@app/models/Pagination';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-evento-lista',
  templateUrl: './evento-lista.component.html',
  styleUrls: ['./evento-lista.component.scss']
})
export class EventoListaComponent implements OnInit {

  modalRef!: BsModalRef;

  public eventos: Evento[] = [];
  public eventoId = 0;
  public pagination = {} as Pagination;

  public widthImg = 150;
  public marginImg = 2;
  public showImg = true;

  termoBuscaChanged: Subject<string> = new Subject<string>();

  // private filtroListado: string = '';

  // public get filtroLista(){
  //   return this.filtroListado;
  // }

  // public set filtroLista(value: string){
  //   this.filtroListado = value;
  //   this.eventosFiltrados = this.filtroLista ? this.filtrarEventos(this.filtroLista) : this.eventos;
  // }

  // public filtrarEventos(filtrarPor: string): Evento[]{
  //   filtrarPor = filtrarPor.toLocaleLowerCase();
  //   return this.eventos.filter(
  //     (evento: any) => evento.tema.toLocaleLowerCase().indexOf(filtrarPor) !== -1 ||
  //     evento.local.toLocaleLowerCase().indexOf(filtrarPor) !== -1
  //   );
  // }

  public filtrarEventos(evento: any): void{
    if(this.termoBuscaChanged.observers.length === 0){
      this.termoBuscaChanged.pipe(debounceTime(1500)).subscribe(
        filtrarPor => {
          this.spinner.show();
          this.eventoService.getEvento(this.pagination.currentPage, this.pagination.itemsPerPage, filtrarPor).subscribe({
            next: (response: PaginatedResult<Evento[]>) => {
              this.eventos = response.result;
              this.pagination = response.pagination;
            },
            error: (error:any) => {
              console.error(error);
              this.spinner.hide(),
              this.toastr.error('Erro ao carregar os eventos', 'Erro');
            },
            complete: () => this.spinner.hide()
          })
        }
      )
    }
    this.termoBuscaChanged.next(evento.value);
  }

  constructor(
      private eventoService: EventoService,
      private modalService: BsModalService,
      private toastr: ToastrService,
      private spinner: NgxSpinnerService
    ) { }

  public ngOnInit(): void {
    this.pagination = {currentPage: 1, itemsPerPage: 3, totalItems: 1} as Pagination;
    this.getEventos();
  }

  public hideImg(): void{
    this.showImg = !this.showImg;
  }

  public mostraImagem(imagemURL: string): string{
    return (imagemURL != '') ? `${environment.apiUrl}resources/images/${imagemURL}` : 'assets/img/semImagem.png';
  }

  public getEventos(): void {
    this.spinner.show();

    this.eventoService.getEvento(this.pagination.currentPage, this.pagination.itemsPerPage).subscribe({
      next: (response: PaginatedResult<Evento[]>) => {
        this.eventos = response.result;
        this.pagination = response.pagination;
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

  pageChanged(event): void{
    this.pagination.currentPage = event.page;
    this.getEventos();
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
