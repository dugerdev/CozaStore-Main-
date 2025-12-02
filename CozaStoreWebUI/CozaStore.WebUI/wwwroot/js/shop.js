// Shop sayfası için JavaScript - Wishlist işlemleri

document.addEventListener('DOMContentLoaded', function() {
    // Tüm wishlist butonlarını bul
    const wishlistButtons = document.querySelectorAll('.js-addwish-b2');
    
    wishlistButtons.forEach(function(button) {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Product ID'yi data attribute'dan al
            const productId = this.getAttribute('data-product-id');
            if (!productId) {
                alert('Ürün ID bulunamadı.');
                return;
            }
            
            // Anti-forgery token al
            const tokenInput = document.querySelector('#antiForgeryForm input[name="__RequestVerificationToken"]');
            if (!tokenInput) {
                // Alternatif olarak sayfadaki herhangi bir form'dan token al
                const form = document.querySelector('form');
                if (form) {
                    const formToken = form.querySelector('input[name="__RequestVerificationToken"]');
                    if (formToken) {
                        addToWishlist(productId, formToken.value, this);
                        return;
                    }
                }
                alert('Güvenlik token bulunamadı.');
                return;
            }
            
            const token = tokenInput.value;
            addToWishlist(productId, token, this);
        });
    });
});

// Wishlist'e ekleme fonksiyonu
function addToWishlist(productId, token, buttonElement) {
    // AJAX ile wishlist'e ekle
    fetch('/Wishlist/Add', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: `productId=${productId}&__RequestVerificationToken=${token}`
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('HTTP error! status: ' + response.status);
        }
        return response.json();
    })
    .then(data => {
        if (data.success) {
            // Başarılı mesajı göster
            alert('Ürün wishlist\'e eklendi!');
            
            // Header'daki wishlist sayısını güncelle
            if (typeof window.updateWishlistIcon === 'function') {
                window.updateWishlistIcon('wishlist-icon', data.count || 0);
                window.updateWishlistIcon('wishlist-icon-mobile', data.count || 0);
            } else {
                // Fallback
                const wishlistIcons = document.querySelectorAll('#wishlist-icon, #wishlist-icon-mobile');
                wishlistIcons.forEach(icon => {
                    icon.setAttribute('data-notify', data.count || 0);
                });
            }
            
            // Wishlist sayısını güncelle (async olarak)
            if (typeof window.updateWishlistCount === 'function') {
                setTimeout(() => {
                    window.updateWishlistCount();
                }, 100);
            }
            
            // Buton görünümünü güncelle (isteğe bağlı)
            if (buttonElement) {
                buttonElement.classList.add('active');
            }
        } else {
            alert('Hata: ' + (data.error || 'Ürün wishlist\'e eklenemedi.'));
        }
    })
    .catch(error => {
        console.error('Error:', error);
        // Sadece gerçek bir hata varsa göster
        if (error.message && !error.message.includes('HTTP error')) {
            alert('Bir hata oluştu. Lütfen tekrar deneyin.');
        }
    });
}

