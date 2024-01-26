import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModalRef?:BsModalRef<ConfirmDialogComponent>;
  constructor(private modalService:BsModalService) { }

  confirm(
    title ='Confirmation',
    message ='Are you sure you want to do this?',
    btnOkText ='Ok',
    btnCancelText = 'Cancel'
  ): Observable<boolean> {
    const config ={
      initialState:{
        title,
        message,
        btnOkText,
        btnCancelText
      }
    }
    // Show the confirmation dialog component as a modal.
    this.bsModalRef = this.modalService.show(ConfirmDialogComponent,config)
    // Return an observable that emits a boolean value when the modal is hidden.
    return this.bsModalRef.onHidden!.pipe(
      map(()=>{
        return this.bsModalRef!.content!.result
      })
    )
  }
}
