import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RegiaoService {
  private baseUrl = 'http://localhost:55700/api';

  constructor(private http: HttpClient) {}

  getCidades(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/cidade`);
  }

  getRegioes(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/regiao`);
  }

  getCidadesPorRegiao(id: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/regiao/id?id=${id}`);
  }

  verificarNomeExistente(nome: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.baseUrl}/regiao/nome?nome=${encodeURIComponent(nome)}`);
  }

  verificarNomeExistenteEditar(nome: string, id: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.baseUrl}/regiao/nome?nome=${encodeURIComponent(nome)}&id=${encodeURIComponent(id)}`);
  }

  salvarRegiao(payload: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/regiao`, payload);
  }
}