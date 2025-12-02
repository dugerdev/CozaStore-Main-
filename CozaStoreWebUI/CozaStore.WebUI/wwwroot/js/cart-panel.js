// Cart Panel dinamik güncelleme

// Sepet panelini yeniden yükle
async function refreshCartPanel() {
    try {
        const response = await fetch('/Cart/GetPanelData');
        if (response.ok) {
            const data = await response.json();
            
            // Items container'ı bul
            const itemsContainer = document.querySelector('#cart-panel-items');
            if (!itemsContainer) return;
            
            // Items'ı güncelle
            if (data.items && data.items.length > 0) {
                itemsContainer.innerHTML = data.items.map(item => `
                    <li class="header-cart-item flex-w flex-t m-b-12">
                        <div class="header-cart-item-img">
                            <a href="/ProductDetail/Index/${item.productId}">
                                <img src="${item.productImageUrl}" alt="${item.productName}">
                            </a>
                        </div>
                        <div class="header-cart-item-txt p-t-8">
                            <a href="/ProductDetail/Index/${item.productId}" class="header-cart-item-name m-b-18 hov-cl1 trans-04">
                                ${item.productName}
                            </a>
                            <span class="header-cart-item-info">
                                ${item.quantity} x $${item.productPrice.toFixed(2)}
                            </span>
                        </div>
                    </li>
                `).join('');
            } else {
                itemsContainer.innerHTML = `
                    <li class="header-cart-item flex-w flex-t m-b-12">
                        <div class="header-cart-item-txt p-t-8 w-full text-center">
                            <span class="stext-102 cl6">
                                Your cart is empty
                            </span>
                        </div>
                    </li>
                `;
            }
            
            // Total'ı güncelle
            const totalContainer = document.querySelector('#cart-panel-total');
            if (totalContainer) {
                totalContainer.innerHTML = `Total: $${data.total.toFixed(2)}`;
            }
        }
    } catch (error) {
        console.error('Cart panel güncellenemedi:', error);
    }
}

// Sepet paneli açıldığında güncelle
document.addEventListener('DOMContentLoaded', function() {
    const cartPanel = document.querySelector('.js-panel-cart');
    
    // Panel açıldığında güncelle (main.js'teki js-show-cart event'inden sonra)
    if (cartPanel) {
        // MutationObserver ile panel açıldığını tespit et
        const observer = new MutationObserver(function(mutations) {
            mutations.forEach(function(mutation) {
                if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
                    if (cartPanel.classList.contains('show-header-cart')) {
                        // Panel açıldı, sepeti güncelle
                        setTimeout(() => {
                            refreshCartPanel();
                        }, 200);
                    }
                }
            });
        });
        
        observer.observe(cartPanel, {
            attributes: true,
            attributeFilter: ['class']
        });
        
        // İlk yüklemede de güncelle (eğer panel açıksa)
        if (cartPanel.classList.contains('show-header-cart')) {
            refreshCartPanel();
        }
    }
});

// Global olarak erişilebilir yap
window.refreshCartPanel = refreshCartPanel;

