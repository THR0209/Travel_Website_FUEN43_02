const app = Vue.createApp({
    data() {
        return {
            promotions: []
        }
    },
    mounted() {
        axios.get('/api/promotions/active')
            .then(res => {
                this.promotions = res.data.map(p => ({
                    id: p.id,
                    title: p.title,
                    imageUrl: p.imageUrl || '/images/default.jpg',
                    price: p.price || 0,
                    meta: p.meta || '台灣・旅遊活動',
                    isTodayOnly: p.isTodayOnly || false
                }))
            })
            .catch(err => console.error(err))
    }
})

app.mount('#promotion-app')
