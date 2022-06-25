import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PaginatedResult, Pagination } from '@app/models/Pagination';
import { Palestrante } from '@app/models/Palestrante';
import { PalestranteService } from '@app/services/palestrante.service';
import { environment } from '@environments/environment';
import { BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-palestrante-lista',
  templateUrl: './palestrante-lista.component.html',
  styleUrls: ['./palestrante-lista.component.scss']
})
export class PalestranteListaComponent implements OnInit {

  public palestrantes: Palestrante[] = [];
  public palestranteId = 0;
  public pagination = {} as Pagination;

  termoBuscaChanged: Subject<string> = new Subject<string>();

  constructor(
      private palestranteService: PalestranteService,
      private modalService: BsModalService,
      private toastr: ToastrService,
      private spinner: NgxSpinnerService,
      private router: Router
    ) { }

  public ngOnInit(): void {
    this.pagination = {currentPage: 1, itemsPerPage: 3, totalItems: 1} as Pagination;
    this.getPalestrantes();
  }

  public getPalestrantes(): void {
    this.spinner.show();

    this.palestranteService.getAllPalestrantes(this.pagination.currentPage, this.pagination.itemsPerPage).subscribe({
      next: (response: PaginatedResult<Palestrante[]>) => {
        this.palestrantes = response.result;
        this.pagination = response.pagination;
      },
      error: (error:any) => {
        console.log(error);
        this.spinner.hide(),
        this.toastr.error('Erro ao carregar os palestrantes', 'Erro');
      },
      complete: () => this.spinner.hide()
    })
  }

  public filtrarPalestrantes(palestrante: any): void{
    if(this.termoBuscaChanged.observers.length === 0){
      this.termoBuscaChanged.pipe(debounceTime(1500)).subscribe(
        filtrarPor => {
          this.spinner.show();
          this.palestranteService.getAllPalestrantes(this.pagination.currentPage, this.pagination.itemsPerPage, filtrarPor).subscribe({
            next: (response: PaginatedResult<Palestrante[]>) => {
              this.palestrantes = response.result;
              this.pagination = response.pagination;
            },
            error: (error:any) => {
              console.error(error);
              this.spinner.hide(),
              this.toastr.error('Erro ao carregar os palestrantes', 'Erro');
            },
            complete: () => this.spinner.hide()
          })
        }
      )
    }
    this.termoBuscaChanged.next(palestrante.value);
  }

  public getImagemURL(imagemName: string): string{
    if(imagemName)
      return imagemName = environment.apiUrl + `resources/perfil/${imagemName}`;
    else
      return imagemName = './assets/img/perfil.jpg';
  }

}
