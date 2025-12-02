// Header Cart ve Wishlist sayaçlarını günceller
(function() {
    'use strict';

    // Cart sayısını güncelle
    function updateCartCount() {
        fetch('/Cart/GetCount')
            .then(response => response.json())
            .then(data => {
                const count = data.count || 0;
                updateCounter('cart-icon', count);
                updateCounter('cart-icon-mobile', count);
                
                // Eğer sepet paneli açıksa, paneli de güncelle
                if (typeof window.refreshCartPanel === 'function') {
                    const cartPanel = document.querySelector('.js-panel-cart');
                    if (cartPanel && cartPanel.classList.contains('show-header-cart')) {
                        window.refreshCartPanel();
                    }
                }
            })
            .catch(error => {
                console.error('Cart count güncellenemedi:', error);
            });
    }

    // Wishlist sayısını güncelle
    function updateWishlistCount() {
        fetch('/Wishlist/GetCount')
            .then(response => response.json())
            .then(data => {
                const count = data.count || 0;
                updateCounter('wishlist-icon', count);
                updateCounter('wishlist-icon-mobile', count);
            })
            .catch(error => {
                console.error('Wishlist count güncellenemedi:', error);
            });
    }

    // Sayaç güncelleme fonksiyonu
    function updateCounter(elementId, count) {
        const element = document.getElementById(elementId);
        if (element) {
            element.setAttribute('data-notify', count);
            // CSS ::after pseudo-element data-notify attribute'unu kullanıyor
            // Attribute değiştiğinde otomatik güncellenir
        }
    }
    
    // Global olarak erişilebilir yap
    window.updateCartIcon = updateCounter;
    window.updateWishlistIcon = updateCounter;

    // Sayfa yüklendiğinde sayaçları güncelle
    document.addEventListener('DOMContentLoaded', function() {
        updateCartCount();
        updateWishlistCount();
    });

    // Global olarak erişilebilir yap
    window.updateCartCount = updateCartCount;
    window.updateWishlistCount = updateWishlistCount;
})();

