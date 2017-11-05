/// <reference path="$nodeModulesRelativePath$/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { $compClassName$ } from './$compClassFileNameWithoutExtension$';

let component: $compClassName$;
let fixture: ComponentFixture<$compClassName$>;

describe('$compName$ component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ $compClassName$ ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent($compClassName$);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});