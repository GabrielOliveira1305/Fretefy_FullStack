import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import Swal from 'sweetalert2';
import * as XLSX from 'xlsx';
import * as FileSaver from 'file-saver';
import { RegiaoService } from './regiao.service';

@Component({
  selector: 'app-regiao',
  templateUrl: './regiao.component.html',
  styleUrls: ['./regiao.component.scss']
})

export class RegiaoComponent implements OnInit {

  regioes: any[] = []; 
  acao: string = '';
  
  cidadesDisponiveis: any[] = [];

  formRegiao!: FormGroup;

  tituloswal: string = '';
  textoswal: string = '';
  tituloswalErro: string = '';
  textoswalErro: string = '';

  constructor(private regiaoService: RegiaoService, private fb: FormBuilder) {}

  ngOnInit(): void {
    this.getCidades();
    this.getRegioes();
    this.criarFormulario();
  }

  criarFormulario() {
    this.formRegiao = this.fb.group({
      id: [''],
      nome: ['', [Validators.required, Validators.maxLength(100)]],
      cidades: this.fb.array([this.criarCidadeControl()], Validators.required)
    });
  }

  criarCidadeControl(cidade?: any): FormGroup {
    return this.fb.group({
      id: [cidade?.id || null, Validators.required],
      nome: [cidade?.nome || '']
    });
  }

  get cidades(): FormArray {
    return this.formRegiao.get('cidades') as FormArray;
  }

  adicionarCidade() {
    this.cidades.push(this.criarCidadeControl());
  }

  removerCidade(index: number) {
    if (this.cidades.length > 1) {
      this.cidades.removeAt(index);
    }
  }

  getCidades(): void {
    this.regiaoService.getCidades().subscribe({
      next: (data) => {
        this.cidadesDisponiveis = data;
      },
      error: (error) => {
        console.error('Erro ao buscar cidades:', error);
      }
    });
  }

  getRegioes(): void {
    this.regiaoService.getRegioes().subscribe({
      next: (data) => {
        this.regioes = data;
        this.carregarCidadesDasRegioes();
      },
      error: (error) => {
        console.error('Erro ao buscar regiões:', error);
      }
    });
  }

  carregarCidadesDasRegioes() {
    this.regioes.forEach(regiao => {
      this.regiaoService.getCidadesPorRegiao(regiao.id).subscribe({
        next: (cidadesDaRegiao) => {
          regiao.cidades = cidadesDaRegiao;
        },
        error: (error) => {
          console.error(`Erro ao buscar cidades da região ${regiao.nome}:`, error);
        }
      });
    });
  }

  setAcao(acao: string) {
    this.acao = acao;
    if (acao === 'Cadastrar') {
      this.formRegiao.reset();
      while (this.cidades.length) {
        this.cidades.removeAt(0);
      }
      this.adicionarCidade();
    }
  }

async salvarRegiao(regiao?: any) {
    if (this.acao === 'Status' && regiao) {
      const payload = {
        regiao: {
          id: regiao.id,
          nome: regiao.nome,
          status: !regiao.status
        },
        idCidades: regiao.cidades?.map((c: any) => c.id) || []
      };

      this.tituloswal = 'Status alterado';
      this.textoswal = 'Status alterado com sucesso.';
      this.tituloswalErro = 'Erro';
      this.textoswalErro = 'Erro ao alterar o status da região.';

      this.enviarPayload(payload);
      return;
    }

    if (this.formRegiao.invalid) {
      this.formRegiao.markAllAsTouched();
      Swal.fire({
        icon: 'error',
        title: 'Formulário inválido',
        text: 'Por favor, corrija os erros antes de salvar.',
        confirmButtonColor: '#3085d6'
      });
      return;
    }

    const cidadesSelecionadas = this.cidades.controls
      .map(c => c.value)
      .filter((c: any) => c?.id != null);

    if (cidadesSelecionadas.length === 0) {
      Swal.fire({
        icon: 'error',
        title: 'Cidade obrigatória',
        text: 'Selecione pelo menos uma cidade para a região',
        confirmButtonColor: '#3085d6'
      });
      return;
    }

    const idsCidades = cidadesSelecionadas.map(c => c.id);
    const idsUnicos = Array.from(new Set(idsCidades));

    if (idsCidades.length !== idsUnicos.length) {
      Swal.fire({
        icon: 'error',
        title: 'Cidades duplicadas',
        text: 'Existem cidades duplicadas na seleção. Por favor, remova as duplicatas.',
        confirmButtonColor: '#3085d6'
      });
      return;
    }

    const nomeTrim = this.formRegiao.value.nome.trim();

    if (this.acao === 'Cadastrar') {
      try {
        const nomeExiste = await this.regiaoService.verificarNomeExistente(nomeTrim).toPromise();
        if (nomeExiste) {
          await Swal.fire({
            icon: 'error',
            title: 'Nome já existe',
            text: 'Já existe uma região com este nome. Por favor, escolha outro nome.',
            confirmButtonColor: '#3085d6'
          });
          return;
        }
      } catch (error) {
        await Swal.fire({
          icon: 'error',
          title: 'Erro de validação',
          text: 'Ocorreu um erro ao validar o nome da região. Por favor, tente novamente.',
          confirmButtonColor: '#3085d6'
        });
        return;
      }
    } else if (this.acao === 'Editar') {
      try {
        const id = this.formRegiao.value.id;
        const nomeExisteEditar = await this.regiaoService.verificarNomeExistenteEditar(nomeTrim, id).toPromise();
        if (nomeExisteEditar) {
          await Swal.fire({
            icon: 'error',
            title: 'Nome já existe',
            text: 'Já existe uma região com este nome. Por favor, escolha outro nome.',
            confirmButtonColor: '#3085d6'
          });
          return;
        }
      } catch (error) {
        await Swal.fire({
          icon: 'error',
          title: 'Erro de validação',
          text: 'Ocorreu um erro ao validar o nome da região. Por favor, tente novamente.',
          confirmButtonColor: '#3085d6'
        });
        return;
      }
    }

    let payload: any;

    if (this.acao === 'Editar') {
      payload = {
        regiao: {
          id: this.formRegiao.value.id,
          nome: nomeTrim,
          status: true
        },
        idCidades: idsUnicos
      };

      this.tituloswal = 'Região editada';
      this.textoswal = 'Região editada com sucesso.';
      this.tituloswalErro = 'Erro';
      this.textoswalErro = 'Erro ao editar região.';
    } else {
      payload = {
        regiao: {
          nome: nomeTrim,
          status: true
        },
        idCidades: idsUnicos
      };

      this.tituloswal = 'Região criada';
      this.textoswal = 'Região criada com sucesso.';
      this.tituloswalErro = 'Erro';
      this.textoswalErro = 'Erro ao criar região.';
    }

    this.enviarPayload(payload);
  }

  private enviarPayload(payload: any) {
    this.regiaoService.salvarRegiao(payload).subscribe({
      next: (response) => {
        if (response.result) {
          this.getRegioes();
          Swal.fire({ icon: 'success', title: this.tituloswal, text: this.textoswal });
          this.formRegiao.reset();
          while (this.cidades.length > 1) {
            this.cidades.removeAt(0);
          }
          document.getElementById('btn-close')?.click();
        } else {
          Swal.fire({ icon: 'error', title: this.tituloswalErro, text: this.textoswalErro });
        }
      },
      error: (err) => {
        console.error('Erro HTTP:', err);
        Swal.fire({ icon: 'error', title: 'Erro', text: 'Erro ao comunicar com o servidor.' });
      }
    });
  }

  editarRegiao(regiao: any) {
    this.setAcao('Editar');
    this.formRegiao.patchValue({
      id: regiao.id,
      nome: regiao.nome
    });

    while (this.cidades.length) {
      this.cidades.removeAt(0);
    }

    if (regiao.cidades && regiao.cidades.length) {
      regiao.cidades.forEach((c: any) => {
        this.cidades.push(this.criarCidadeControl(c));
      });
    } else {
      this.adicionarCidade();
    }

    const modalElement = document.getElementById('cadastrarModal');
    const modal = new (window as any).bootstrap.Modal(modalElement);
    modal.show();
  }

  exportarExcel() {
    if (!this.regioes.length) {
      Swal.fire({
        icon: 'warning',
        title: 'Nenhum dado',
        text: 'Não há dados para exportar',
        confirmButtonColor: '#3085d6'
      });
      return;
    }

    const exportData = this.regioes.map(regiao => ({
      ID: regiao.id,
      Nome: regiao.nome,
      Status: regiao.status ? 'Ativo' : 'Inativo',
      Cidades: regiao.cidades?.map((c: any) => c.nome).join(', ') || ''
    }));

    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(exportData);
    const workbook: XLSX.WorkBook = { Sheets: { 'Regiões': worksheet }, SheetNames: ['Regiões'] };
    const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    const data: Blob = new Blob([excelBuffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8' });
    FileSaver.saveAs(data, 'Regioes.xlsx');
  }
}
