import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Palestrante } from '@app/models/Palestrante';
import { PalestranteService } from '@app/services/palestrante.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { debounceTime, map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-palestrante-detalhe',
  templateUrl: './palestrante-detalhe.component.html',
  styleUrls: ['./palestrante-detalhe.component.scss']
})
export class PalestranteDetalheComponent implements OnInit {

  public form!: FormGroup;
  public situacaoForm = '';
  public corDescricao = '';

  public get f(): any{
    return this.form.controls;
  }

  constructor(private fb: FormBuilder, public palestranteService: PalestranteService, public toastr: ToastrService, public spinner: NgxSpinnerService) { }

  ngOnInit() {
    this.validation();
    this.verificaForm();
    this.carregarPalestrante();
  }

  public validation(): void{
    this.form = this.fb.group({
      miniCurriculo: ['']
    })
  }

  private verificaForm(): void{
    this.form.valueChanges.pipe(map(() => {
      this.situacaoForm = 'Minicurrículo está sendo atualizado..';
      this.corDescricao = 'text-warning';
      }),
      debounceTime(1500),
      tap(() => this.spinner.show())
    )
    .subscribe({
      next: () => {
        this.palestranteService.put({ ...this.form.value }).subscribe({
          next: () => {
            this.situacaoForm = 'Minicurrículo foi atualizado!'
            this.corDescricao = 'text-success'

            setTimeout(() => {
              this.situacaoForm = 'Minicurrículo carregado!'
              this.corDescricao = 'text-muted'
            }, 2000);
          },
          error: () => {
            this.toastr.error('Erro ao tentar atualizar palestrante', 'Erro');
          },
          complete: () => {
            this.spinner.hide();
          }
        })
      }
    })
  }

  private carregarPalestrante(): void{
    this.spinner.show();

    this.palestranteService.getPalestrante().subscribe({
      next: (palestrante: Palestrante) => {
        this.form.patchValue(palestrante);
      },
      error: () => {
        this.toastr.error('Erro ao tentar carregar palestrante', 'Erro');
      },
      complete: () => {
        this.spinner.hide();
      }
    })
  }

}
