import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms'; 
import { RegiaoComponent } from './regiao.component';
import { RegiaoRoutingModule } from './regiao.routing';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RegiaoRoutingModule
  ],
  declarations: [RegiaoComponent],
  exports: [RegiaoComponent]
})
export class RegiaoModule { }
