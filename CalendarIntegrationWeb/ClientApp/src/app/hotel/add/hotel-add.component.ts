import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-hotel-add',
  templateUrl: './hotel-add.component.html',
})
export class HotelAddComponent {
  public hotelId;

  constructor(private route: ActivatedRoute) {
    this.hotelId = this.route.snapshot.params.id;
  }
}
