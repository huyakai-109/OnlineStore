import { Component, Input } from '@angular/core';
import { Product } from '../../../shared/Models/product';
import {
    MatCard,
    MatCardActions,
    MatCardContent,
} from '@angular/material/card';
import { CurrencyPipe } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';

@Component({
    selector: 'app-product-item',
    standalone: true,
    imports: [
        MatCard,
        MatCardContent,
        CurrencyPipe,
        MatCardActions,
        MatButton,
        MatIcon,
    ],
    templateUrl: './product-item.component.html',
    styleUrl: './product-item.component.scss',
})
export class ProductItemComponent {
    @Input() product?: Product;
}
