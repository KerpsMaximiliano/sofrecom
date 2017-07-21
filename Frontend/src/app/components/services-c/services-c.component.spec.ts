import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServicesCComponent } from './services-c.component';

describe('ServicesCComponent', () => {
  let component: ServicesCComponent;
  let fixture: ComponentFixture<ServicesCComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServicesCComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServicesCComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should be created', () => {
    expect(component).toBeTruthy();
  });
});
