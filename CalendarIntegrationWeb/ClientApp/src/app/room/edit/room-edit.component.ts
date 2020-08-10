import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-room-add',
  templateUrl: './room-edit.component.html',
})
export class RoomEditComponent {
  public roomId;

  constructor(private route: ActivatedRoute) {
    this.roomId = this.route.snapshot.params.id;
  }
}
