import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-hotel-list',
  templateUrl: './hotel-list.component.html',
})
export class HotelListComponent {
  public hotelId;

  constructor(private route: ActivatedRoute) {
    this.hotelId = this.route.snapshot.params.id;
  }
}
