// Cart functionality for shopping cart page

document.addEventListener('DOMContentLoaded', function() {
    // Quantity increase/decrease buttons
    const btnNumProductDown = document.querySelectorAll('.btn-num-product-down');
    const btnNumProductUp = document.querySelectorAll('.btn-num-product-up');
    const btnUpdateCart = document.getElementById('btnUpdateCart');

    // Decrease quantity
    btnNumProductDown.forEach(function(btn) {
        btn.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation(); // main.js'teki event'i durdur
            
            const cartItemId = this.getAttribute('data-cart-item-id');
            if (!cartItemId) return; // Sadece cart sayfasındaki butonlar için çalış
            
            const input = document.querySelector('input[data-cart-item-id="' + cartItemId + '"]');
            if (!input) return;
            
            let currentValue = parseInt(input.value) || 1;
            
            if (currentValue > 1) {
                currentValue--;
                input.value = currentValue;
                updateCartItemQuantity(cartItemId, currentValue);
            }
        });
    });

    // Increase quantity
    btnNumProductUp.forEach(function(btn) {
        btn.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation(); // main.js'teki event'i durdur
            
            const cartItemId = this.getAttribute('data-cart-item-id');
            if (!cartItemId) return; // Sadece cart sayfasındaki butonlar için çalış
            
            const input = document.querySelector('input[data-cart-item-id="' + cartItemId + '"]');
            if (!input) return;
            
            let currentValue = parseInt(input.value) || 1;
            
            currentValue++;
            input.value = currentValue;
            updateCartItemQuantity(cartItemId, currentValue);
        });
    });

    // Update Cart button - updates all quantities
    if (btnUpdateCart) {
        btnUpdateCart.addEventListener('click', function() {
            updateAllQuantities();
        });
    }
});

// Update single cart item quantity
function updateCartItemQuantity(cartItemId, quantity) {
    if (!cartItemId || quantity < 1) {
        return;
    }

    // Find existing form or create new one
    let form = document.querySelector('form[data-cart-item-id="' + cartItemId + '"]');
    
    if (!form) {
        // Create form and submit
        form = document.createElement('form');
        form.method = 'POST';
        form.action = '/Cart/UpdateQuantity';
        form.style.display = 'none';
        form.setAttribute('data-cart-item-id', cartItemId);
        
        // Add anti-forgery token if available
        const antiForgeryForm = document.getElementById('antiForgeryForm');
        if (antiForgeryForm) {
            const token = antiForgeryForm.querySelector('input[name="__RequestVerificationToken"]');
            if (token) {
                const tokenInput = document.createElement('input');
                tokenInput.type = 'hidden';
                tokenInput.name = '__RequestVerificationToken';
                tokenInput.value = token.value;
                form.appendChild(tokenInput);
            }
        }

        document.body.appendChild(form);
    }

    // Update or create hidden inputs
    let cartItemIdInput = form.querySelector('input[name="cartItemId"]');
    if (!cartItemIdInput) {
        cartItemIdInput = document.createElement('input');
        cartItemIdInput.type = 'hidden';
        cartItemIdInput.name = 'cartItemId';
        form.appendChild(cartItemIdInput);
    }
    cartItemIdInput.value = cartItemId;

    let quantityInput = form.querySelector('input[name="quantity"]');
    if (!quantityInput) {
        quantityInput = document.createElement('input');
        quantityInput.type = 'hidden';
        quantityInput.name = 'quantity';
        form.appendChild(quantityInput);
    }
    quantityInput.value = quantity;

    // Submit form
    form.submit();
}

// Update all cart item quantities
function updateAllQuantities() {
    const quantityInputs = document.querySelectorAll('input[data-cart-item-id][name="quantity"]');
    
    quantityInputs.forEach(function(input) {
        const cartItemId = input.getAttribute('data-cart-item-id');
        const quantity = parseInt(input.value) || 1;
        
        if (quantity > 0) {
            updateCartItemQuantity(cartItemId, quantity);
        }
    });
}

// Sepete ürün eklendiğinde header'daki cart sayısını güncelle
if (typeof window.updateCartCount === 'function') {
    window.updateCartCount();
}

