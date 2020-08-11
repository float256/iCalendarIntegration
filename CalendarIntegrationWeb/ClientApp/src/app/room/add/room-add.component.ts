import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-room-add',
  templateUrl: './room-add.component.html',
})
export class RoomAddComponent {
  public hotelId;

  constructor(private route: ActivatedRoute) {
    this.hotelId = this.route.snapshot.params.id;
  }
}
