import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-hotel-delete',
  templateUrl: './room-delete.component.html',
})
export class RoomDeleteComponent {
  public roomId;

  constructor(private route: ActivatedRoute) {
    this.roomId = this.route.snapshot.params.id;
  }
}
